using UnityEngine;

[CreateAssetMenu(fileName = "WallStats", menuName = "Scriptable Objects/WallStats")]
public class WallStats : ScriptableObject
{
    public float hp = 6f;
    public float objBuildTime = 0.5f;
    public int objBuildCount = 6;
}
