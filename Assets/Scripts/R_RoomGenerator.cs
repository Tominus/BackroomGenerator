using System.Collections;
using UnityEngine;

public class R_RoomGenerator : S_Singleton<R_RoomGenerator>
{
    [SerializeField] R_Room roomPrefab;
    [SerializeField] Transform worldSocket;
    [SerializeField, Range(1, 100)] int roomWallDestroyChance = 75;
    [SerializeField] int genSeed = 0;
    [SerializeField, Range(0.01f, 9.99f)] float genFrequency = 0.99f;
    
    void Start()
    {
        if (!worldSocket)
        {
            Debug.Log("No world socket for rooms");
            return;
        }
    }

    public R_Room GenerateRoom(Vector3Int vector3Int)
    {
        R_Room _room = Instantiate(roomPrefab, vector3Int, Quaternion.identity, worldSocket);

        GameObject _wallNorth = _room.wallNorth;
        GameObject _wallSouth = _room.wallSouth;
        GameObject _wallEast = _room.wallEast;
        GameObject _wallWest = _room.wallWest;

        Vector3 _wallNorthPos = _wallNorth.transform.position;
        Vector3 _wallSouthPos = _wallSouth.transform.position;
        Vector3 _wallEastPos = _wallEast.transform.position;
        Vector3 _wallWestPos = _wallWest.transform.position;

        float _roomWallDestroyChance = roomWallDestroyChance / 100f;

        if (PerlinNoise(_wallNorthPos.x, _wallNorthPos.z) < _roomWallDestroyChance)
            _wallNorth.SetActive(false);
        if (PerlinNoise(_wallSouthPos.x, _wallSouthPos.z) < _roomWallDestroyChance)
            _wallSouth.SetActive(false);
        if (PerlinNoise(_wallEastPos.x, _wallEastPos.z) < _roomWallDestroyChance)
            _wallEast.SetActive(false);
        if (PerlinNoise(_wallWestPos.x, _wallWestPos.z) < _roomWallDestroyChance)
            _wallWest.SetActive(false);

        return _room;
    }

    public float PerlinNoise(float _x, float _z)
    {
        return Mathf.PerlinNoise(_x * genFrequency + genSeed, _z * genFrequency + genSeed);
    }
}