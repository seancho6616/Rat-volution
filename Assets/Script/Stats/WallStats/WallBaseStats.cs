using UnityEngine;

[CreateAssetMenu(fileName = "WallBaseStats", menuName = "Scriptable Objects/WallBaseStats")]
public class WallBaseStats : ScriptableObject
{
    public int hp = 6;
    public float objBuildTime = 5.0f;
    public int objBuildCount = 6;
    public float reBuildTime = 5.0f;
}
