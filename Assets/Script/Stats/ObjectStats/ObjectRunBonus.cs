using UnityEngine;

public class ObjectRunBonus
{
    public float hp;
    public float minSpawnTime;
    public float mixSpawnTime;
    public float fallingTime;
    public float objBuildCount;
    public float livingTime;
    public float reBuildTime;

    public void Reset()
    {
        hp = 0;
        minSpawnTime =0;
        mixSpawnTime = 0;
        fallingTime = 0;
        objBuildCount = 0;
        livingTime = 0;
        reBuildTime = 0;
    }
    public void LevelUP(int level)
    {
        if (level % 3 == 0)
        {
            hp += 10f;
            minSpawnTime -= 0.5f;
            mixSpawnTime -= 0.5f;
        }
        if (level % 2 == 0)
        {
            fallingTime -= 0.1f;
        }
        if (level % 4 == 0)
        {
            livingTime -= 0.5f;
        }
    }
}
