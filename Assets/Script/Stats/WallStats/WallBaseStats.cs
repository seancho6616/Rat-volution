using UnityEngine;

[CreateAssetMenu(fileName = "WallBaseStats", menuName = "Scriptable Objects/WallBaseStats")]
public class WallBaseStats : ScriptableObject
{
    public int hp = 6;
    public float objBuildTime = 0.5f;
    public int objBuildCount = 6;
}
