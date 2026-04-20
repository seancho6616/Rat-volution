using UnityEngine;

public enum ItemType{ Active, OneTime }
[CreateAssetMenu(fileName = "CardItemData", menuName = "Scriptable Objects/CardItemData")]
public class CardItemData : BaseCardData
{
    public ItemType itemType;
    public float      scalePerStack;  // 중첩 당 증가 수치
    public float      maxStack;       // 최대 중첩 수 

    
#if UNITY_EDITOR
    private void OnValidate()
    {
        cardType = CardType.Item; // 자동 고정

        // if (maxStack <= 0)
        //     Debug.LogWarning($"[CardItemData] '{cardName}': maxStack이 0 이하입니다.", this);
        if (amount <= 0)
            Debug.LogWarning($"[CardItemData] '{cardName}': amount가 0 이하입니다.", this);
    }
#endif
}
