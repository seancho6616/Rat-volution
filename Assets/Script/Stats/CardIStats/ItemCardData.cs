using UnityEngine;

public enum ItemType{ Active, OneTime }
[CreateAssetMenu(fileName = "ItemCardData", menuName = "Scriptable Objects/ItemCardData")]
public class ItemCardData : ScriptableObject
{
    public int      amount;         // 증가 수치
    public int      scalePerStack;  // 중첩 당 증가 수치
    public int      maxStack;       // 최대 중첩 수 
    public ItemType itemType;
}
