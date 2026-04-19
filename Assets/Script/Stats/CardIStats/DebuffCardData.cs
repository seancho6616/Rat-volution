using UnityEngine;

/*  디버프 카드 데이터
DebuffType - 0~4 고양이발 관련 타입 & 5~6 벽 관련 타입
*/
public enum DebuffType{ 
    ObjHp, ObjSpawnTime, ObjWarningTime, ObjLivingTime, ObjReBuildTime,
    WallHp, WallBuildTime,
    }

[CreateAssetMenu(fileName = "DebuffCardData", menuName = "Scriptable Objects/DebuffCardData")]
public class DebuffCardData : BaseCardData
{
    public DebuffType   debuffType;     // 디버프 타입
    public float        minAmount;      // 최소값
    public float        maxAmount;      // 최대값


#if UNITY_EDITOR
private void OnValidate()
{
    cardType = CardType.Debuff; // 자동 고정

    if (minAmount > maxAmount)
        Debug.LogWarning($"[DebuffCardData] '{cardName}': minAmount가 maxAmount보다 큽니다.", this);
}
#endif
}
