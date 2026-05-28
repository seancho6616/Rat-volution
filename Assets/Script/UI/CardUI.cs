using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum CardMode { Reward, Inventory, Popup }

    [Header("UI 연결")]
    public GameObject Motion; 
    public GameObject frontView; 
    public GameObject backView;  
    public Image imgIcon;
    public TMP_Text txtName;
    public TMP_Text txtRarity;
    public TMP_Text txtDesc;
    public TMP_Text txtDebuff;

    [Header("도감용")]
    public GameObject unknownCard; // 미획득 시 표시할 물음표 가림막

    [Header("이펙트 제어")]
    public CardGlowController glowController; 

    private BaseCardData currentData;
    public CardDebuffData cardDebuffData;
    private CardMode currentMode = CardMode.Reward;
    private bool isDiscovered = true;
    private bool isFlipped = false;
    private Coroutine flipCoroutine;
    private float flipDuration = 0.25f;

    void Start()
    {   
        
    }
    // 카드 데이터 설정
    public void SetCardData(BaseCardData data, CardDebuffData debuffData, CardMode mode = CardMode.Reward, bool discovered = true)
    {
        currentData = data;
        currentMode = mode;
        isDiscovered = discovered;
        cardDebuffData = debuffData;

        if (txtDebuff != null)
        {
            if (cardDebuffData != null) txtDebuff.text = debuffData.description;
            else txtDebuff.text = "";
        }

        if (imgIcon != null) imgIcon.sprite = data.icon;
        if (txtName != null) txtName.text = data.cardName;
        if (txtRarity != null) txtRarity.text = data.cardRarity.ToString();
        if (txtDesc != null) txtDesc.text = data.description;

        Motion.transform.rotation = Quaternion.identity;
        isFlipped = false;

        if (glowController != null)
        {
            bool shouldShowGlow = (currentMode == CardMode.Reward || currentMode == CardMode.Popup);
            glowController.SetupGlow(data.cardRarity, shouldShowGlow);
        }

        if (currentMode == CardMode.Inventory)
        {
            // 도감 모드
            frontView.SetActive(isDiscovered);
            backView.SetActive(false);
            if (unknownCard != null) unknownCard.SetActive(!isDiscovered);
        }
        else
        {
            // 보상 및 팝업 모드
            frontView.SetActive(true);
            backView.SetActive(false);
            if (unknownCard != null) unknownCard.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // [기획 반영 2] 보상 모드일 때만 마우스 올리면 뒤집어짐 (도감 모드는 반응 안 함)
        if (currentMode == CardMode.Reward)
        {
            StopFlip();
            flipCoroutine = StartCoroutine(RotateCard(180f));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 보상 모드일 때만 마우스 떼면 원상복구
        if (currentMode == CardMode.Reward)
        {
            StopFlip();
            flipCoroutine = StartCoroutine(RotateCard(0f));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentMode == CardMode.Reward)
        {
            // [보상 모드] 카드 선택
            //Debug.Log(gameObject.name + " 선택됨!");
            PlayerStats.Instance.ApplyCard(currentData); 
            if(cardDebuffData != null) PlayerStats.Instance.ApplyCard(cardDebuffData);
            //RewardUIManager.Instance.HideUIGameObj(RewardUIManager.Instance.pickCardGroup);

            // 카드 선택 보상
            if (CardManager.Instance != null)
            {
                CardManager.Instance.OnCardClick(currentData);
            }
        }
        else if (currentMode == CardMode.Inventory)
        {
            if (isDiscovered && CardInventoryUI.Instance != null)
            {
                CardInventoryUI.Instance.OpenCardPopup(currentData);
            }
        }
        else if (currentMode == CardMode.Popup)
        {
            // [팝업 모드 클릭] 팝업창 한가운데 뜬 카드 클릭 시 앞/뒤로 뒤집힘!
            float targetAngle = isFlipped ? 0f : 180f;
            StopFlip();
            flipCoroutine = StartCoroutine(RotateCard(targetAngle));
            isFlipped = !isFlipped;
        }
    }

    // 회전 코루틴 정지
    private void StopFlip()
    {
        if (flipCoroutine != null) StopCoroutine(flipCoroutine);
    }

    // 회전 애니메이션 로직 (인게임 보상창 전용 또는 팝업창에서 재활용)
    IEnumerator RotateCard(float targetY)
    {
        float elapsed = 0f;
        Quaternion startRot = Motion.transform.rotation;
        Quaternion endRot = Quaternion.Euler(0, targetY, 0);

        while (elapsed < flipDuration)
        {
            elapsed += Time.unscaledDeltaTime; 
            float progress = elapsed / flipDuration;
            Motion.transform.rotation = Quaternion.Slerp(startRot, endRot, progress);

            float currentY = Motion.transform.rotation.eulerAngles.y;

            if (currentY > 90f && currentY < 270f)
            {
                frontView.SetActive(false);
                backView.SetActive(true);
            }
            else
            {
                frontView.SetActive(true);
                backView.SetActive(false);
            }
            yield return null;
        }
        Motion.transform.rotation = endRot;
    }
}