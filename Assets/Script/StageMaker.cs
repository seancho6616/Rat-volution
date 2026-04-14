using UnityEngine;

public class StageMaker : StageData
{
    void Start()
    {
        transform.localScale = new Vector3(
            finalGridSizeCount*baseData.gridSize, 
            -4f,
            finalGridSizeCount * baseData.gridSize);
    }
}
