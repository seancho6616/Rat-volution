using Unity.Collections;
using UnityEngine;

public class StageMaker : StageData
{
    public static StageMaker Instance;
    [SerializeField]
    private Material groundMaterial;

    protected override void Awake()
    {
        if(Instance ==null) Instance = this;
        else    Destroy(gameObject);
        base.Awake();
    }
    void Start()
    {
        transform.localScale = new Vector3(
            finalGridSizeCount*baseData.gridSize, 
            transform.localScale.y,
            finalGridSizeCount * baseData.gridSize);
        SetTiling();    
    }
    public void GridSizeUP(int level)
    {
        if(level %5 == 0)
        {
            finalGridSizeCount += baseData.gridSizeCountPerLevel;
            finalObjGridSizeCount += baseData.gridSizeCountPerLevel;
            SizeUP();
        }
    }

    private void SizeUP()
    {
        transform.localScale = new Vector3(
            finalGridSizeCount*baseData.gridSize, 
            transform.localScale.y,
            finalGridSizeCount * baseData.gridSize);
        SetTiling();
    }

    private void SetTiling()
    {
        groundMaterial.mainTextureScale = new Vector2(finalGridSizeCount, finalGridSizeCount);
    }
}
