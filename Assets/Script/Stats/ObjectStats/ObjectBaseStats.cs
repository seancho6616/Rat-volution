using UnityEngine;

[CreateAssetMenu(fileName = "ObjectBaseStats", menuName = "Scriptable Objects/ObjectBaseStats")]
public class ObjectBaseStats : ScriptableObject
{
    public float hp = 15f;
    public float minSpawnTime = 3f;
    public float mixSpawnTime = 4f;
    public float minWarningTime = 1.5f;
    public float mixWarningTime = 2f;
    public float fallingTime = 1f;
    public int objBuildCount = 3;
    public float livingTime = 3f;
    public float reBuildTime = 5f;
}
