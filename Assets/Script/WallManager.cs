using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum WallType { Horizontal, Vertical }

[System.Serializable] 
public class WallData
{
    public Vector3 position;      // 벽이 생성된 기준 좌표 (중복 생성 방지 체크용)
    public WallType type;         // 가로(Horizontal) 또는 세로(Vertical) 타입
    public GameObject wallObject; // 씬에 실제로 생성된 벽 게임 오브젝트
}

public class WallManager : WallStats
{
    public static WallManager Instance;
    public List<WallData> activeWalls = new List<WallData>();
    [Header("Prefab")]
    public GameObject wallPrefab;

    [Header("Settings")]
    [SerializeField] private float wallSecound =5f;
    private const int MAxWallCount = 6; // 최대 벽 개수

    private List<Vector3> point = new List<Vector3>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        point = new List<Vector3>(SpawnPointManager.Instance.objectsSpawnPositions);
        //Debug.Log(point.Count);
        StartCoroutine(SpawnWallRoutine());
    }

    private IEnumerator SpawnWallRoutine()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(wallSecound);
            if (activeWalls.Count < MAxWallCount)
            {
                SpawnWallAtRandom();
            }
        }
    }
    private void SpawnWallAtRandom()
    {
        if (point.Count == 0) return;

        Vector3 spawnPos = point[Random.Range(0, point.Count)];

        WallType selectedType = (Random.value > 0.5f) ? WallType.Horizontal : WallType.Vertical;

        bool isDuplicate = activeWalls.Any(w => w.position == spawnPos && w.type == selectedType);
        if (isDuplicate) return;

        Quaternion rotation = (selectedType == WallType.Vertical) ? Quaternion.Euler(0, 90f, 0) : Quaternion.identity;

        Vector3 adjustedPos = spawnPos;
        if (selectedType == WallType.Vertical)
        {
            adjustedPos.z += 0.5f;
        }
        else
        {
            adjustedPos.x += 0.5f;
        }

        GameObject wallObj = Instantiate(wallPrefab, adjustedPos, rotation);
        if (wallObj.GetComponent<Wall>() != null)
        {
            wallObj.GetComponent<Wall>().Init(spawnPos);
        }
        activeWalls.Add(new WallData { position = spawnPos, type = selectedType, wallObject = wallObj });
    }
    public void ReleaseWall(GameObject wallObj)
    {
        WallData data = activeWalls.Find(w => w.wallObject == wallObj);
        if (data != null)
        {
            activeWalls.Remove(data);
        }
    }

    public void InvsetWallStatPoint(DebuffType type, float amount)
    {
        switch (type)
        {
            case DebuffType.WallHp:
                runBonus.hp += (int)amount;
                break;
            case DebuffType.WallBuildTime:
                runBonus.objBuildTime += amount;
                break;
        }
    }
}
