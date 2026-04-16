using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    public GameObject itemPrefab;
    public int itemSpawnCount = 0;

    private List<Vector3> point = new List<Vector3>();
    private Dictionary<Vector3, bool> itemDictionary = new Dictionary<Vector3, bool>();

    private void Awake()
    {
        if (Instance == null)
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
        point = new List<Vector3>(SpawnPointManager.Instance.itemSpawnPositions);
        foreach (Vector3 pos in point)
        {
            itemDictionary[pos] = true;
            GameObject obj = Instantiate(itemPrefab, pos, Quaternion.identity);
            itemSpawnCount++;
        }
        StartCoroutine(ItemSpawn());
    }


    private IEnumerator ItemSpawn()
    {
        while (true)
        {
            yield return new WaitUntil(() => itemSpawnCount == 0);
            ResetItemDictionary();
            NewMethod();
        }
    }

    private void NewMethod()
    {
        int count = Random.Range(21, 35);
        List<Vector3> emptyPoint = new List<Vector3>();
        // count 값만큼 반복을 사용하는데 아이템이 없는 랜덤한 포인트에 아이템이 있으면 다시 랜덤으로 포인트 생성
        for (int a = 0; a <= count; a++)
        {
            foreach(var kvp in itemDictionary)
            {
                if(!kvp.Value) emptyPoint.Add(kvp.Key);
            }
            if(emptyPoint.Count ==0) return;
            Vector3 vector3 = emptyPoint[Random.Range(0, emptyPoint.Count)];
            itemDictionary[vector3] = true;
            GameObject obj = Instantiate(itemPrefab, vector3, Quaternion.identity);
            itemSpawnCount++;
        }
    }

    private void ResetItemDictionary()
    {
        foreach (Vector3 pos in point)
        {
            itemDictionary[pos] = false;
        }
    }
}
