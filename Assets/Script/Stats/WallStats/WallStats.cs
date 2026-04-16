using UnityEngine;

public class WallStats : MonoBehaviour
{
    public WallBaseStats baseData;
    public WallRunBonus runBonus = new WallRunBonus();
    public int Finalhp => baseData.hp + runBonus.hp;
    public float FinalObjBuildTime => baseData.objBuildTime + runBonus.objBuildTime;
    public int FinalObjBuildCount =>  baseData.objBuildCount + runBonus.objBuildCount;
}
