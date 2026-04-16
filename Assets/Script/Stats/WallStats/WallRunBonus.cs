using UnityEngine;

public class WallRunBonus
{
    public int hp;
    public float objBuildTime;
    public int objBuildCount;

    public void Reset()
    {
        hp = 0;
        objBuildTime = 0;
        objBuildCount = 0;
    }

    public void LevelUP(int level)
    {
        if (level % 3 == 0)
        {
            hp += 4;
            objBuildTime += 0.5f;
            objBuildCount += 3;
        }
    }
}
