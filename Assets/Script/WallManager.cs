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

    [Header("Collision Check")]
    public LayerMask objectLayer;
    public LayerMask wallLayer;
    public float checkRadius = 4.5f;

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

    public int GetMaxWallCount()
    {
        int currentLevel = PlayerStats.Instance != null ? PlayerStats.Instance.level : 1;

        if (currentLevel <= 5) return 6;
        if (currentLevel <= 10) return 8;
        if (currentLevel <= 15) return 10;
        if (currentLevel <= 20) return 15;
        return 20; // 레벨 21 이상에서는 최대 20개까지 생성 가능
    }

    public float GetWallSpawnInterval()
    {
        int currentLevel = PlayerStats.Instance != null ? PlayerStats.Instance.level : 1;

        if (currentLevel <= 5) return 5.0f;
        if (currentLevel <= 10) return 4.0f;
        if (currentLevel <= 15) return 3.0f;
        if (currentLevel <= 20) return 2.0f;
        return 1.5f; // 레벨 21 이상에서는 1.5초마다 생성
    }

    private IEnumerator SpawnWallRoutine()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(GetWallSpawnInterval());
            if (activeWalls.Count < GetMaxWallCount())
            {
                SpawnWallAtRandom();
            }
        }
    }
    private void SpawnWallAtRandom()
    {
        if (point.Count == 0) return;

        Vector3 spawnPos = point[Random.Range(0, point.Count)];

        // 물리적 충돌 체크 
        if (Physics.CheckSphere(spawnPos, checkRadius, objectLayer))
        {
            return; // 충돌이 감지되면 벽 생성하지 않음
        }

        bool isDuplicatePos = activeWalls.Any(w => w.position == spawnPos);
        if (isDuplicatePos) return;

        WallType selectedType = (Random.value > 0.5f) ? WallType.Horizontal : WallType.Vertical;
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

        Vector3 halfExtents = (selectedType == WallType.Vertical) ? new Vector3(1f, 2f, 4.5f) : new Vector3(4.5f, 2f, 1f);
        if (Physics.CheckBox(adjustedPos, halfExtents, rotation, wallLayer))
        {
            return; // 충돌이 감지되면 벽 생성하지 않음
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
