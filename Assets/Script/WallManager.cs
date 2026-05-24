using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        return FinalObjBuildCount;
    }

    public float GetWallSpawnInterval()
    {
        return FinalObjBuildTime;
    }

    private IEnumerator SpawnWallRoutine()
    {
        float timer = 0f;
        while (true)
        {
            
            timer += Time.deltaTime;
            if (timer >= GetWallSpawnInterval())
            {
                if (activeWalls.Count < GetMaxWallCount())
                {
                    SpawnWallAtRandom();
                }
                timer = 0f;
            }
            yield return null;
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
    public void LevelUP(int level)
    {
        runBonus.LevelUP(level);
        point.Clear();   
        point = new List<Vector3>(SpawnPointManager.Instance.objectsSpawnPositions);
        
    }
}
