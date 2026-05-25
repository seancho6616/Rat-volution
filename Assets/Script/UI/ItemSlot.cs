using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using JetBrains.Annotations;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI 연결")]
    public Image image;
    public Image itemIcon;
    public TMP_Text itemCountTxt;
    public TMP_Text itemNameTxt;

    [HideInInspector]
    public string cardID;         // 중복 획득 여부를 확인할 카드 고유 이름
    private int currentCount;     // 카드 획득 개수
    private float amount;
    float specialMoveTimer = 0f;  
    float specialMoveMaxCooldown = 0f;

    void Start()
    {
        // 시작할 때는 이름이 안 보이도록 함
        if (itemNameTxt != null) itemNameTxt.text = "";
    }

    void Update()
    {
        if (cardID == "SpecialMove" && specialMoveTimer > 0f)
        {
            Debug.Log($"[1] Timer 감소 중: {specialMoveTimer}");
            
            specialMoveTimer -= Time.deltaTime;
            if (specialMoveTimer < 0f) specialMoveTimer = 0f;

            float ratio = specialMoveMaxCooldown > 0f 
                ? specialMoveTimer / specialMoveMaxCooldown 
                : 0f;
            Debug.Log($"[2] ratio = {ratio}");
            UpdateCooldown(ratio);
        }
    }

    // 카드를 처음 획득했을 때 슬롯을 초기화
    public void SetItemSlot(BaseCardData data)
    {
        cardID = data.cardName;
        currentCount += 1;
        image.gameObject.SetActive(false);
        switch (cardID)
        {
            case "Magnet":
            case "AdrenalineRush":
            case "SlowMotion":
            case "DealwithTheDevil":
                break;
            default:
                amount = data.amount;
                break;
        }

        if (itemIcon != null)
        {
            itemIcon.sprite = data.icon; // 실제 카드 아이콘 적용
            itemIcon.gameObject.SetActive(true);
        }

        UpdateCountText();
    }

    // 이미 있는 카드를 또 먹었을 때 개수만 올려주는 함수
    public void AddItemCount(CardItemData data)
    {
        Dictionary<CardItemData, float> itemCheck = Inventory.Instance.itemCheck;
        float currentStack = itemCheck.ContainsKey(data) ? itemCheck[data] : 0;
        image.gameObject.SetActive(false);
        if (currentStack >= data.maxStack)
        {
            Debug.Log("최대 중복수 도달");
            return;
        }
        else
        {
            currentCount += 1;            
        }
        switch (data.cardName)
        {
            case "DisposableShield":
                amount = Inventory.Instance.item.shield;
                break;
            case "SharpFangs":
                amount = Inventory.Instance.item.sharpFangs;
                break;
            case "DoT":
                amount = Inventory.Instance.item.dot;
                break;
            case "RapidStrike":
                amount = Inventory.Instance.item.rapidStrike;
                break;
            case "LuckySeven":
                amount = Inventory.Instance.item.luckySeven;
                break;
            case "SpecialMove":
                amount = Inventory.Instance.item.specialMove;
                break;
            default:
                break;
        }
        
        UpdateCountText();
    }

    // 텍스트 업데이트 로직
    public void UpdateCountText()
    {
        if (itemCountTxt != null)
        {
            itemCountTxt.text = currentCount.ToString();
        }
    }

    // 마우스 올렸을 때 카드 이름 출력
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemNameTxt != null)
        {
            itemNameTxt.text = cardID + " : " + amount.ToString();
        }
    }

    //마우스 나갔을 때 카드 이름 삭제
    public void OnPointerExit(PointerEventData eventData)
    {
        if (itemNameTxt != null)
        {
            itemNameTxt.text = "";
        }
    }
    
    public void RefreshAmount()
    {
        switch (cardID)
        {
            case "DisposableShield":
                amount = Inventory.Instance.item.shield;
                if(amount <= 0)
                {
                    Debug.Log("색변경");
                    image.gameObject.SetActive(true);
                    image.color = new Color32(127, 127,127, 127);
                }
                break;
            case "SharpFangs":
                amount = Inventory.Instance.item.sharpFangs;
                break;
            case "DoT":
                amount = Inventory.Instance.item.dot;
                break;
            case "RapidStrike":
                amount = Inventory.Instance.item.rapidStrike;
                break;
            case "LuckySeven":
                amount = Inventory.Instance.item.luckySeven;
                break;
            case "SpecialMove":
                amount = Inventory.Instance.item.specialMove;
                break;
            case "SlowMotion":
                if (!Inventory.Instance.item.slowMotion)
                {
                    image.gameObject.SetActive(true);
                    image.color = new Color32(127, 127,127, 127);
                }
                break;
            case "DealwithTheDevil":
                if (!Inventory.Instance.item.dealWithDevil)
                {
                    Debug.Log("색변경");
                    image.gameObject.SetActive(true);
                    image.color = new Color32(127, 127, 127, 127);
                }
                break;
            default:
                break;
        }
    }

    public void StartCooldown()
    {
        specialMoveMaxCooldown = Inventory.Instance.item.specialMove;
        specialMoveTimer = specialMoveMaxCooldown;
    }
    public void UpdateCooldown(float ratio)
    {
        if (image == null) return;

        if (ratio <= 0f)
        {
            image.gameObject.SetActive(false);
        }
        else
        {
            image.gameObject.SetActive(true);
            image.fillAmount = ratio;
        }
    }
}
