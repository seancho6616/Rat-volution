using System.Collections.Generic;
using UnityEngine;

public class ItemSlotManager : MonoBehaviour
{
    public static ItemSlotManager Instance;

    [Header("아이템 슬롯 UI")]
    public GameObject itemSlotPrefab; // ItemSlot 프리팹
    public Transform itemSlotList;
    public GameObject itemSlotGroup;
    
    // 현재 생성된 슬롯들을 관리하는 리스트
    private List<ItemSlot> activeSlots = new List<ItemSlot>();

    void Start()
    {
        if (itemSlotGroup != null)
        {
            itemSlotGroup.SetActive(false);
        }
    }

    void Awake()
    {
        // 싱글톤 세팅
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 카드를 획득했을 때 호출할 함수
    public void AddCardSlot(BaseCardData data)
    {
        if (data.cardType != CardType.Item)
        {
            return; 
        }

        if (itemSlotGroup != null)
        {
            itemSlotGroup.SetActive(true);
        }
        // 1. 이미 획득한 카드인지 확인 (이름으로 검사)
        ItemSlot existingSlot = activeSlots.Find(slot => slot.cardID == data.cardName);

        if (existingSlot != null)
        {
            // 2-A. 이미 있다면 개수만 증가
            existingSlot.AddItemCount((CardItemData)data);
        }
        else
        {
            // 2-B. 처음 획득한 카드라면 프리팹 새로 생성
            if (itemSlotPrefab != null && itemSlotList != null)
            {
                GameObject newSlotObj = Instantiate(itemSlotPrefab, itemSlotList);
                ItemSlot newSlot = newSlotObj.GetComponent<ItemSlot>();
                
                if (newSlot != null)
                {
                    newSlot.SetItemSlot(data); // 아이콘, 이름 등 초기 세팅
                    activeSlots.Add(newSlot);  // 리스트에 추가하여 다음 중복 검사에 대비
                }
            }
        }
    }
    public void RefreshItemSlot(string cardName)
    {
        ItemSlot targetSlot = activeSlots.Find(slot => slot.cardID == cardName);
        if (targetSlot != null)
        {
            targetSlot.RefreshAmount();
        }
    }

    public void StartSpecialMoveCooldown()
    {
        ItemSlot slot = activeSlots.Find(s => s.cardID == "SpecialMove");
        if (slot != null) slot.StartCooldown();
    }
}