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
    private bool spawnPaused = false;

    private void Awake() => Instance = this;

    private void Start()
    {
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
        // 7x7 그리드 내에서 2x2가 들어갈 랜덤 좌표 선정 (0~5 사이의 인덱스)
        int xIdx = Random.Range(-1, 2);
        while(xIdx == 0)
        {
            xIdx = Random.Range(-1,2);
        }
        int zIdx = Random.Range(-1, 2);
        while(zIdx == 0)
        {
            zIdx = Random.Range(-1,2);
        }
        Transform transform = player.transform;
        
        // 유닛 사이즈 10을 곱해 실제 좌표 계산 (중심점 보정 +5)
        Vector3 spawnPos = new Vector3(xIdx*5f + Mathf.Round(transform.position.x), 0, 
        zIdx*5f + Mathf.Round(transform.position.z));

        GameObject obj = Instantiate(objectPrefab, spawnPos, Quaternion.identity);
        obj.GetComponent<FallingObject>().Init(10f, unitSize); // HP 10 전달
        activeObjects.Add(obj);
    }

    public void OnObjectRemoved(GameObject obj, bool byPlayer)
    {
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