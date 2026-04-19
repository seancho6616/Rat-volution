using UnityEngine;

public enum ItemType{ Active, OneTime }
[CreateAssetMenu(fileName = "ItemCardData", menuName = "Scriptable Objects/ItemCardData")]
public class ItemCardData : BaseCardData
{
    public ItemType itemType;
    public int      scalePerStack;  // 중첩 당 증가 수치
    public int      maxStack;       // 최대 중첩 수 

    
#if UNITY_EDITOR
    private void OnValidate()
    {
        cardType = CardType.Item; // 자동 고정

        if (maxStack <= 0)
            Debug.LogWarning($"[ItemCardData] '{cardName}': maxStack이 0 이하입니다.", this);
        if (amount <= 0)
            Debug.LogWarning($"[ItemCardData] '{cardName}': amount가 0 이하입니다.", this);
    }
#endif
}
