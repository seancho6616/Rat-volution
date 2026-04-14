using UnityEngine;

[CreateAssetMenu(fileName = "StageBaseStats", menuName = "Scriptable Objects/StageBaseStats")]
public class StageBaseStats : ScriptableObject
{
    [Header("기본 데이터")]
    public int gridSizeCount = 7;
    public int objGridSizeCount = 6;
    public int gridSize = 10;

    [Header("레벨 업 시 올라가는 데이터")]
    public int gridSizeCountPerLevel = 2;
}
