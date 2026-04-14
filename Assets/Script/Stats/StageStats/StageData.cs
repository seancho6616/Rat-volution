using UnityEngine;
using UnityEngine.EventSystems;

public class StageData : MonoBehaviour
{
    public static StageData Instance;
    public StageBaseStats baseData;
    public int finalGridSizeCount;
    public int finalObjGridSizeCount;
    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
        finalGridSizeCount = baseData.gridSizeCount;
        finalObjGridSizeCount = baseData.objGridSizeCount;
    }
    public void GridSizeUP(int level)
    {
        if(level /5 == 0)
        {
            finalGridSizeCount += baseData.gridSizeCountPerLevel;
            finalObjGridSizeCount += baseData.gridSizeCountPerLevel;
        }
    }
}
