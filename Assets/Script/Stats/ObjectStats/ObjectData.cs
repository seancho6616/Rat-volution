using UnityEngine;

public class ObjectData : MonoBehaviour
{
    public ObjectBaseStats baseData;
    public ObjectRunBonus runBonus = new ObjectRunBonus();
    public int Finalhp => baseData.hp + runBonus.hp;
    public float FinalMinSpawnTime => baseData.minSpawnTime + runBonus.minSpawnTime;
    public float FinalMixSpawnTime => baseData.mixSpawnTime + runBonus.mixSpawnTime;
    public float FinalMinWarningTime => baseData.minWarningTime + runBonus.minWarningTime;
    public float FinalMixWarningTime => baseData.mixWarningTime + runBonus.mixWarningTime;
    public float FinalFallingTime => baseData.fallingTime + runBonus.mixSpawnTime;
    public int   FinalObjBuildCount =>baseData.objBuildCount + (int)runBonus.objBuildCount;
    public float FinalLivingTime => baseData.livingTime + runBonus.livingTime;
    public float FinalReBuildTime => baseData.reBuildTime + runBonus.reBuildTime;
}
