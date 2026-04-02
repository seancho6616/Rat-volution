using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    public static WallManager Instance;
    public GameObject wallPrefab;
    [SerializeField] private float wallSecound =10f;

    private List<Vector3> point = new List<Vector3>();
    private Dictionary<Vector3, bool> wallSpawnDictionary = new Dictionary<Vector3, bool>();

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
        Debug.Log(point.Count);
        foreach(Vector3 pos in point)
        {
            wallSpawnDictionary[pos] = false;
            Debug.Log(wallSpawnDictionary[pos]);
        }
        StartCoroutine(SpawnWallRoutine());
    }

    private IEnumerator SpawnWallRoutine()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(wallSecound);
            SpawnWallAtRandom();
        }
    }
    private void SpawnWallAtRandom()
    {
        List<Vector3> emptyPoints = new List<Vector3>();
        foreach (var kvp in wallSpawnDictionary)
            if (!kvp.Value) emptyPoints.Add(kvp.Key);

        if (emptyPoints.Count == 0)
        {
            Debug.LogWarning("[WallManager] 벽을 스폰할 빈 공간이 없습니다.");
            return;
        }
        Vector3 spawnPos = emptyPoints[Random.Range(0, emptyPoints.Count)];

        // Y축 rotation 0 또는 180 랜덤
        Quaternion rotation = Quaternion.Euler(0, Random.value > 0.5f ? 180f : 0f, 0);

        GameObject wallObj = Instantiate(wallPrefab, spawnPos, rotation);
        wallObj.GetComponent<Wall>().Init(spawnPos);
        
        wallSpawnDictionary[spawnPos] = true;

        Debug.Log($"[WallManager] 벽 스폰: {spawnPos}");
    }
    public void ReleaseWall(Vector3 pos)
    {
        if (wallSpawnDictionary.ContainsKey(pos))
            wallSpawnDictionary[pos] = false;
    }
}
