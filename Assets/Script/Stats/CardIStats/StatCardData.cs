using UnityEngine;

[CreateAssetMenu(fileName = "StatCardData", menuName = "Scriptable Objects/StatCardData")]
public class StatCardData : BaseCardData
{
    public StatType statType;


#if UNITY_EDITOR
    private void OnValidate()
    {
        cardType = CardType.StatUp; // 자동 고정

        if (statType == StatType.None)
            Debug.LogWarning($"[StatCardData] '{cardName}': StatType이 설정되지 않았습니다.", this);
        if (amount <= 0)
            Debug.LogWarning($"[StatCardData] '{cardName}': amount가 0 이하입니다.", this);
    }
#endif
}
