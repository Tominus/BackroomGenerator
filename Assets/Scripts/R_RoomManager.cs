using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class R_RoomManager : S_Singleton<R_RoomManager>
{
    [SerializeField] R_RoomGenerator roomGenerator;
    [SerializeField] P_Player player;
    Transform playerTransform = null;
    [SerializeField, Range(1, 100)] int roomViewDistance = 5;
    [SerializeField, Range(1, 100)] int roomSize = 10;
    [SerializeField] Vector3Int playerStartPosition = Vector3Int.zero;

    R_Room[][] allRooms = null;
    int totalRoomViewDistance = 0;
    [SerializeField] Vector3Int arrayWorldPosition = Vector3Int.zero;

    [SerializeField] bool isGeneratingRooms = false;
    [SerializeField] bool hasPlayerMoved = false;

    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

    public Vector3Int CalculatePlayerPosition()
    {
        Vector3 _tmpPosition = playerTransform.position / roomSize;
        return new Vector3Int(Mathf.FloorToInt(_tmpPosition.x), Mathf.FloorToInt(_tmpPosition.y), Mathf.FloorToInt(_tmpPosition.z));
    }

    private void Start()
    {
        roomGenerator = R_RoomGenerator.Instance;
        player = P_Player.Instance;

        InitPlayerPosition();
        InitRoomArray();
        InitSpawnRoom();
    }

    void InitPlayerPosition()
    {
        playerTransform = player.transform;
        playerTransform.position = playerStartPosition;
        hasPlayerMoved = true;
    }
    void InitRoomArray()
    {
        totalRoomViewDistance = 1 + 2 * roomViewDistance;

        allRooms = new R_Room[totalRoomViewDistance][];
        for (int i = 0; i < totalRoomViewDistance; ++i)
        {
            allRooms[i] = new R_Room[totalRoomViewDistance];
        }

        Vector3Int _playerPosition = CalculatePlayerPosition();
        arrayWorldPosition = new Vector3Int(_playerPosition.x - roomViewDistance,
                                            _playerPosition.y,
                                            _playerPosition.z - roomViewDistance);
    }
    void InitSpawnRoom()
    {
        Vector3Int _playerRoomPosition = CalculatePlayerPosition();
        GenerateRooms(_playerRoomPosition);
    }

    private void Update()
    {
        UpdateRooms();
    }

    void UpdateRooms()
    {
        if (isGeneratingRooms) return;

        Vector3Int _playerRoomPosition = CalculatePlayerPosition();

        CheckPlayerMovement(_playerRoomPosition);
        GenerateRooms(_playerRoomPosition);
    }

    void CheckPlayerMovement(Vector3Int _playerRoomPosition)
    {
        if (_playerRoomPosition.x != arrayWorldPosition.x + roomViewDistance || 
            _playerRoomPosition.z != arrayWorldPosition.z + roomViewDistance)
        {
            //calculate offset

            Vector3Int _moveOffset = new Vector3Int(_playerRoomPosition.x - (arrayWorldPosition.x + roomViewDistance),
                                                    0,
                                                    _playerRoomPosition.z - (arrayWorldPosition.z + roomViewDistance));

            //delete oob rooms

            /*int _startDeleteX = 0;
            int _endDeleteX = 0;

            int _startDeleteZ = 0;
            int _endDeleteZ = 0;

            for (int x = 0; x < length; ++x)
            {

            }*/



            //move others room






            //recenter player

            arrayWorldPosition.x += _moveOffset.x;
            arrayWorldPosition.z += _moveOffset.z;

            hasPlayerMoved = true;
        }
        else
        {
            hasPlayerMoved = false;
        }
    }
    void GenerateRooms(Vector3Int _playerRoomPosition)
    {
        if (!hasPlayerMoved) return;

        List<Vector3Int> _roomsPositionToGenerate = new List<Vector3Int>();
        
        //Check which room to generate

        for (int x = 0; x < totalRoomViewDistance; ++x)
        {
            R_Room[] _rooms = allRooms[x];

            for (int z = 0; z < totalRoomViewDistance; ++z)
            {
                R_Room _room = _rooms[z];
                if (!_room)
                {
                    _roomsPositionToGenerate.Add(new Vector3Int(x, 0, z));
                }
            }
        }

        if (_roomsPositionToGenerate.Count > 0)
        {
            isGeneratingRooms = true;
            StartCoroutine(nameof(RequestRoomGeneration), _roomsPositionToGenerate);
        }
    }

    IEnumerator RequestRoomGeneration(List<Vector3Int> _roomsPositionToGenerate)
    {
        // Ask RoomGenerator to generate rooms
        int _count = _roomsPositionToGenerate.Count;
        for (int i = 0; i < _count; ++i)
        {
            Vector3Int _roomPosition = _roomsPositionToGenerate[i];
            R_Room _room = roomGenerator.GenerateRoom(new Vector3Int((_roomPosition.x + arrayWorldPosition.x) * roomSize,
                                                                      0,
                                                                     (_roomPosition.z + arrayWorldPosition.z) * roomSize));
            if (!_room)
            {
                Debug.LogError("No room generated");
                continue;
            }

            allRooms[_roomPosition.x][_roomPosition.z] = _room;

            yield return waitForEndOfFrame;
        }

        isGeneratingRooms = false;
    }
}