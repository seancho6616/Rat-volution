using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    public GameObject itemPrefab;

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
        foreach(Vector3 pos in point)
        {
            itemDictionary[pos] = true;
            GameObject obj = Instantiate(itemPrefab, pos, Quaternion.identity);
        }
    }
}
