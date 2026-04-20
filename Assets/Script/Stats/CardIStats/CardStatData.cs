using UnityEngine;

[CreateAssetMenu(fileName = "CardStatData", menuName = "Scriptable Objects/CardStatData")]
public class CardStatData : BaseCardData
{
    public StatType statType;


#if UNITY_EDITOR
    private void OnValidate()
    {
        cardType = CardType.StatUp; // 자동 고정

        if (statType == StatType.None)
            Debug.LogWarning($"[CardStatData] '{cardName}': StatType이 설정되지 않았습니다.", this);
        if (amount <= 0)
            Debug.LogWarning($"[CardStatData] '{cardName}': amount가 0 이하입니다.", this);
    }
#endif
}
