using System.Collections;
using UnityEngine;

public class R_RoomGenerator : S_Singleton<R_RoomGenerator>
{
    [SerializeField] R_Room roomPrefab;
    [SerializeField] Transform worldSocket;
    [SerializeField, Range(1, 100)] int roomWallDestroyChance = 75;

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

        if (Random.Range(0, 101) < roomWallDestroyChance)
            _room.wallNorth.SetActive(false);
        if (Random.Range(0, 101) < roomWallDestroyChance)
            _room.wallSouth.SetActive(false);
        if (Random.Range(0, 101) < roomWallDestroyChance)
            _room.wallEast.SetActive(false);
        if (Random.Range(0, 101) < roomWallDestroyChance)
            _room.wallWest.SetActive(false);

        return _room;
    }
}