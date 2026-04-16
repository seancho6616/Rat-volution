using UnityEngine;

/*  디버프 카드 데이터
DebuffType - 0~4 고양이발 관련 타입 & 5~6 벽 관련 타입
*/
public enum DebuffType{ 
    ObjHp, ObjSpawnTime, ObjWarningTime, ObjLivingTime, ObjReBuildTime,
    WallHp, WallBuildTime,
    }

[CreateAssetMenu(fileName = "DebuffCardData", menuName = "Scriptable Objects/DebuffCardData")]
public class DebuffCardData : ScriptableObject
{
    public float        minAmount;      // 최소값
    public float        mixAmount;      // 최대값
    public DebuffType   debuffType;     // 디버프 타입
}
