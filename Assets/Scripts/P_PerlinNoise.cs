using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct PerlinNoiseSettings
{
    [SerializeField] public double frequency;
    [SerializeField] public double octaves;
    [SerializeField] public double resolution;

    public PerlinNoiseSettings(double _frequency, double _octaves, double _resolution)
    {
        frequency = _frequency;
        octaves = _octaves;
        resolution = _resolution;
    }
}

public class P_PerlinNoise : S_Singleton<P_PerlinNoise>
{
    [SerializeField] private int seed = 0;
    [SerializeField] private double perlinNoiseDefaultZ = 3.1415;
    [SerializeField] private PerlinNoiseSettings defaultSettings;
    [SerializeField] private PerlinNoiseSettings biomeSettings;

    private int[] permutation = null;

    protected override void Awake()
    {
        base.Awake();
        InitSeed();
        Shuffle();
        GenerateImage();
    }
    private void InitSeed()
    {
        Random.InitState(seed);

        permutation = Enumerable.Range(0, 256).ToArray();
    }
    private void Shuffle()
    {
        int _count = permutation.Length;
        while (_count > 0)
        {
            int _random = Random.Range(0, _count);

            int _tmp = permutation[_count - 1];
            permutation[_count - 1] = permutation[_random];
            permutation[_random] = _tmp;

            --_count;
        }
    }
    private void GenerateImage()
    {

    }

    public double GetDefaultNoise(double _x, double _y)
    {
        return ClampNoiseResult(CalculateOctave(_x * defaultSettings.frequency,
                                                _y * defaultSettings.frequency,
                                                defaultSettings.octaves)) * defaultSettings.resolution;
    }
    public double GetBiomeNoise(double _x, double _y)
    {
        return ClampNoiseResult(CalculateOctave(_x * biomeSettings.frequency,
                                                _y * biomeSettings.frequency,
                                                biomeSettings.octaves)) * biomeSettings.resolution;
    }

    private double CalculateOctave(double _x, double _y, double _octaves)
    {
        double _result = 0.0;
        double _amplitude = 1.0;

        for (int i = 0; i < _octaves; ++i)
        {
            _result += GetNoise(_x, _y) * _amplitude;
            _x *= 2;
            _y *= 2;
            _amplitude *= 0.5;
        }

        return _result;
    }
    private double ClampNoiseResult(double _x) 
    {
        if (_x <= -1.0)
        {
            return 0.0;
        }
        else if (_x > 1.0)
        {
            return 1.0;
        }
        return _x * 0.5 + 0.5;
    }
    private double GetNoise(double x, double y)
    {
        double _x = Math.Floor(x);
        double _y = Math.Floor(y);
        double _z = Math.Floor(perlinNoiseDefaultZ);

        int _ix = (int)_x & 255;
        int _iy = (int)_y & 255;
        int _iz = (int)_z & 255;

        double _fx = (x - _x);
        double _fy = (y - _y);
        double _fz = (perlinNoiseDefaultZ - _z);

        double _u = Fade(_fx);
        double _v = Fade(_fy);
        double _w = Fade(_fz);

        uint _A = (uint)(permutation[_ix & 255] + _iy) & 255;
        uint _B = (uint)(permutation[(_ix + 1) & 255] + _iy) & 255;

        uint _AA = (uint)(permutation[_A] + _iz) & 255;
        uint _AB = (uint)(permutation[(_A + 1) & 255] + _iz) & 255;

        uint _BA = (uint)(permutation[_B] + _iz) & 255;
        uint _BB = (uint)(permutation[(_B + 1) & 255] + _iz) & 255;

        double _p0 = Grad((uint)permutation[_AA], _fx, _fy, _fz);
        double _p1 = Grad((uint)permutation[_BA], _fx - 1.0, _fy, _fz);
        double _p2 = Grad((uint)permutation[_AB], _fx, _fy - 1.0, _fz);
        double _p3 = Grad((uint)permutation[_BB], _fx - 1.0, _fy - 1.0, _fz);
        double _p4 = Grad((uint)permutation[(_AA + 1) & 255], _fx, _fy, _fz - 1.0);
        double _p5 = Grad((uint)permutation[(_BA + 1) & 255], _fx - 1.0, _fy, _fz - 1.0);
        double _p6 = Grad((uint)permutation[(_AB + 1) & 255], _fx, _fy - 1.0, _fz - 1.0);
        double _p7 = Grad((uint)permutation[(_BB + 1) & 255], _fx - 1.0, _fy - 1.0, _fz - 1.0);

        double _q0 = Lerp(_p0, _p1, _u);
        double _q1 = Lerp(_p2, _p3, _u);
        double _q2 = Lerp(_p4, _p5, _u);
        double _q3 = Lerp(_p6, _p7, _u);

        double _r0 = Lerp(_q0, _q1, _v);
        double _r1 = Lerp(_q2, _q3, _v);

        return Lerp(_r0, _r1, _w);
    }
    private double Fade(double _t)
    {
        return _t * _t * _t * (_t * (_t * 6.0 - 15.0) + 10.0);
    }
    private double Grad(uint _hash,  double _x, double _y, double _z)
    {
        uint _h = _hash & 15;
        double _u = _h < 8 ? _x : _y;
        double _v = _h < 4 ? _y : _h == 12 || _h == 14 ? _x : _z;
        return ((_h & 1) == 0 ? _u : -_u) + ((_h & 2) == 0 ? _v : -_v);
    }
    private double Lerp(double _a, double _b, double _t)
    {
        return _a + (_b - _a) * _t;
    }
}