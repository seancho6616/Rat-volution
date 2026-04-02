using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class SpawnPointManager : MonoBehaviour
{
    public static SpawnPointManager Instance;
    [SerializeField] private int gridSize = 7;
    [SerializeField] private int objectgridSize = 6;
    [SerializeField] private float cellSize = 10f;

    public readonly List<Vector3> objectsSpawnPositions = new List<Vector3>();
    public List<Vector3> itemSpawnPositions = new List<Vector3>();

    public enum ObjectType
    {
        Empty, Item, Object, Well,
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        GenerateGridItem();
        GenerateGridObject();
    }

    private void GenerateGridItem()
    {
        itemSpawnPositions.Clear();
        int half = gridSize / 2;
        for (int x = -half; x <= half; x++)
        {
            for (int z = -half; z <= half; z++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0, z * cellSize);
                itemSpawnPositions.Add(pos);
            }
        }
    }

    private void GenerateGridObject()
    {
        objectsSpawnPositions.Clear();
        int half = objectgridSize / 2;
        for (int x = -half; x < half; x++)
        {
            for (int z = -half; z < half; z++)
            {
                Vector3 pos = new Vector3((x * cellSize) + 5f, 0, (z * cellSize) + 5f);
                objectsSpawnPositions.Add(pos);
            }
        }
        string result = string.Join(",", objectsSpawnPositions);
    }
}
