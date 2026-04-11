using UnityEngine;

[CreateAssetMenu(fileName = "ObjectStats", menuName = "Scriptable Objects/ObjectStats")]
public class ObjectStats : ScriptableObject
{
    public float hp = 15f;
    public float minSpawnTime = 3f;
    public float mixSpawnTime = 4f;
    public float fallingTime = 1f;
    public int objBuildCount = 3;
    public float livingTime = 3f;
    public float reBuildTime = 5f;
}
