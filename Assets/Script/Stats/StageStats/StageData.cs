using UnityEngine;
using UnityEngine.EventSystems;

public class StageData : MonoBehaviour
{
    public static StageData Instance;
    public StageBaseStats baseData;
    public int finalGridSizeCount;
    public int finalObjGridSizeCount;
    protected virtual void Awake()
    {
        finalGridSizeCount = baseData.gridSizeCount;
        finalObjGridSizeCount = baseData.objGridSizeCount;
    }
    public void LevelUP(int level)
    {
        if(level % 5 == 0)
        {
            finalGridSizeCount += baseData.gridSizeCountPerLevel;
            finalObjGridSizeCount += baseData.gridSizeCountPerLevel;
        }
    }
    
}
