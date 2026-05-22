using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI 연결")]
    public Image itemIcon;
    public TMP_Text itemCountTxt;
    public TMP_Text itemNameTxt;

    [HideInInspector]
    public string cardID;         // 중복 획득 여부를 확인할 카드 고유 이름
    private float currentCount;

    void Start()
    {
        // 시작할 때는 이름이 안 보이도록 함
        if (itemNameTxt != null) itemNameTxt.text = "";
    }

    // 카드를 처음 획득했을 때 슬롯을 초기화
    public void SetItemSlot(BaseCardData data)
    {
        cardID = data.cardName;
        switch (cardID)
        {
            case "Magnet":
            case "AdrenalineRush":
            case "SlowMotion":
            case "DealwithTheDevil":
                currentCount += 1;
                break;
            default:
                currentCount = data.amount;
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
    public void AddItemCount(BaseCardData data)
    {
        switch (data.cardName)
        {
            case "DisposableShield":
                currentCount = Inventory.Instance.item.shield;
                break;
            case "SharpFangs":
                currentCount = Inventory.Instance.item.sharpFangs;
                break;
            case "DoT":
                currentCount = Inventory.Instance.item.dot;
                break;
            case "RapidStrike":
                currentCount = Inventory.Instance.item.rapidStrike;
                break;
            case "LuckySeven":
                currentCount = Inventory.Instance.item.luckySeven;
                break;
            case "SpecialMove":
                currentCount = Inventory.Instance.item.specialMove;
                break;
            default:
                break;
        }
        UpdateCountText();
    }

    // 텍스트 업데이트 로직
    private void UpdateCountText()
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
            itemNameTxt.text = cardID;
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
    
}
