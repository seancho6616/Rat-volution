using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance;
    [SerializeField] private GameObject player;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int gridSizeCount = 7; // 7x7
    public float unitSize = 1f;  // 한 칸의 길이 10
    
    public GameObject objectPrefab;
    private List<GameObject> activeObjects = new List<GameObject>();
    private List<Vector3> point = new List<Vector3>();
    private Dictionary<Vector3, bool> objectDictionary = new Dictionary<Vector3, bool>();
    private bool spawnPaused = false;

    private void Awake()
    {
        if(Instance ==null) Instance =this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        point = new List<Vector3>(SpawnPointManager.Instance.objectsSpawnPositions);
        foreach(Vector3 pos in point)
        {
            objectDictionary[pos] =false;
        }
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // 레벨 1 기준 3~4초 간격 생성
            yield return new WaitForSeconds(Random.Range(3f, 4f));

            // 생성 조건: 유예 기간이 아니고, 최대 개수(3개) 미만일 때
            if (!spawnPaused && activeObjects.Count < 3)
            {
                SpawnFallingObject();
            }
        }
    }

    private void SpawnFallingObject()
    {
        Vector2 playerIdx = GetPlayerPointIndex();
        List<Vector3> candidates = new List<Vector3>
        {
            new Vector3(playerIdx.x + 5f, 0, playerIdx.y + 5f),
            new Vector3(playerIdx.x + 5f, 0, playerIdx.y - 5f),
            new Vector3(playerIdx.x - 5f, 0, playerIdx.y + 5f),
            new Vector3(playerIdx.x - 5f, 0, playerIdx.y - 5f)
        };

        List<Vector3> validCandidates = candidates.FindAll(c => 
        point.Contains(c) && objectDictionary.ContainsKey(c) && !objectDictionary[c]);
        if (validCandidates.Count == 0)
        {
            Debug.Log("유효한 스폰 위치 없음");
            return;
        }

        // 유닛 사이즈 10을 곱해 실제 좌표 계산 (중심점 보정 +5)
        Vector3 spawnPos = validCandidates[Random.Range(0, validCandidates.Count)];
        objectDictionary[spawnPos] = true;
        GameObject obj = Instantiate(objectPrefab, spawnPos, Quaternion.identity);
        obj.GetComponent<FallingObject>().Init(10f, unitSize); // HP 10 전달
        activeObjects.Add(obj);
    }

    private Vector2 GetPlayerPointIndex()
    {
        int cellSize = (int)SpawnPointManager.Instance.cellSize;
        int xIdx = Mathf.RoundToInt(player.transform.position.x / cellSize);
        int zIdx = Mathf.RoundToInt(player.transform.position.z / cellSize);
        return new Vector2Int(xIdx*cellSize, zIdx*cellSize);
    }

    public void OnObjectRemoved(GameObject obj, bool byPlayer)
    {
        objectDictionary[obj.transform.position] = false;
        activeObjects.Remove(obj);
        if (byPlayer)
        {
            StartCoroutine(RespawnDelayRoutine());
        }
    }

    private IEnumerator RespawnDelayRoutine()
    {
        spawnPaused = true;
        Debug.Log("플레이어가 물체 파괴! 5초간 생성 중단");
        yield return new WaitForSeconds(5f);
        spawnPaused = false;
    }
}