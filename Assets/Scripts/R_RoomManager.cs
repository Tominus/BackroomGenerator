using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class R_RoomManager : S_Singleton<R_RoomManager>
{
    [SerializeField] R_RoomGenerator roomGenerator;
    [SerializeField] P_Player player;
    [SerializeField, Range(1, 100)] int roomViewDistance = 5;
    [SerializeField, Range(1, 100)] int roomSize = 10;
    [SerializeField] Vector3Int playerStartPosition = Vector3Int.zero;

    R_Room[][] allRooms = null;
    int totalRoomViewDistance = 0;
    [SerializeField] bool isGeneratingRooms = false;
    [SerializeField] bool hasPlayerMoved = false;

    [SerializeField] Vector3Int arrayWorldPosition = Vector3Int.zero;
    Transform playerTransform = null;

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
    }

    void InitPlayerPosition()
    {
        playerTransform = player.transform;
        playerTransform.position = playerStartPosition;
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
            //delete oob rooms
            //move others room
            //recenter player
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

        if (_roomsPositionToGenerate.Count > 0)
        {
            isGeneratingRooms = true;
            StartCoroutine(nameof(RequestRoomGeneration), _roomsPositionToGenerate);
        }
    }

    IEnumerator RequestRoomGeneration(List<Vector3Int> _roomsPositionToGenerate)
    {
        // Ask RoomGenerator to generate rooms


        isGeneratingRooms = false;
        yield return null;
    }
}