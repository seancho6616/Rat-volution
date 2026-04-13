using UnityEngine;

[CreateAssetMenu(fileName = "StageStats", menuName = "Scriptable Objects/StageStats")]
public class StageStats : ScriptableObject
{
    public int gridSizeCount = 7;
    public int objGridSizeCount => gridSizeCount -1;
    public int gridSize = 10;
}
