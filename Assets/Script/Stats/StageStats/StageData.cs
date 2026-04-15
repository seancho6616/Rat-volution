using UnityEngine;
using UnityEngine.EventSystems;

public class StageData : MonoBehaviour
{
    public StageBaseStats baseData;
    public int finalGridSizeCount;
    public int finalObjGridSizeCount;
    protected virtual void Awake()
    {
        finalGridSizeCount = baseData.gridSizeCount;
        finalObjGridSizeCount = baseData.objGridSizeCount;
    }
    
}
