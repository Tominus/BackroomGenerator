using System.Collections;
using UnityEngine;

public class R_RoomGenerator : S_Singleton<R_RoomGenerator>
{
    [SerializeField] R_Room roomPrefab;
    [SerializeField] Transform worldSocket;
    [SerializeField, Range(1, 100)] int roomCount = 20;
    [SerializeField, Range(1, 100)] int roomSize = 10;
    [SerializeField, Range(1, 100)] int roomWallDestroyChance = 75;

    WaitForEndOfFrame frame = new WaitForEndOfFrame();

    void Start()
    {
        if (!worldSocket)
        {
            Debug.Log("No world socket for rooms");
            return;
        }
        StartCoroutine(nameof(SpawnRoom), this);
    }

    IEnumerator SpawnRoom()
    {
        int minRoomPosition = roomSize * -roomCount;
        int roomPositionX = minRoomPosition;
        int roomPositionZ = minRoomPosition;

        for (int i = -roomCount; i < roomCount; ++i)
        {
            for (int j = -roomCount; j < roomCount; ++j)
            {
                R_Room room = Instantiate(roomPrefab, new Vector3(roomPositionX, 0, roomPositionZ), Quaternion.identity, worldSocket);

                if (Random.Range(0, 101) < roomWallDestroyChance)
                    room.wallNorth.SetActive(false);
                if (Random.Range(0, 101) < roomWallDestroyChance)
                    room.wallSouth.SetActive(false);
                if (Random.Range(0, 101) < roomWallDestroyChance)
                    room.wallEast.SetActive(false);
                if (Random.Range(0, 101) < roomWallDestroyChance)
                    room.wallWest.SetActive(false);

                roomPositionZ += roomSize;
            }

            roomPositionZ = minRoomPosition;
            roomPositionX += roomSize;
        }

        yield return frame;
    }
}