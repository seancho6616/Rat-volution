using UnityEngine;

public class StageData : MonoBehaviour
{
    public StageBaseStats baseStats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(baseStats.objGridSizeCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
