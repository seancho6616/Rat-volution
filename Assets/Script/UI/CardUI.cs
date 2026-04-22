using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI 연결")]
    public GameObject Motion; // 회전시킬 오브젝트
    public GameObject frontView; // FrontView 연결
    public GameObject backView;  // BackView 연결
    public Image imgIcon;
    public TMP_Text txtName;
    public TMP_Text txtRarity;
    public TMP_Text txtDesc;

    private BaseCardData currentData;
    private bool isFlipped = false;
    private Coroutine flipCoroutine;
    private float flipDuration = 0.25f;

    // 카드 데이터 설정
    public void SetCardData(BaseCardData data)
    {
        currentData = data;
        if (imgIcon != null) imgIcon.sprite = data.icon;
        if (txtName != null) txtName.text = data.cardName;
        if (txtRarity != null) txtRarity.text = data.cardRarity.ToString();
        //if (txtDesc != null) txtDesc.text = data.description;
        
        // 초기 상태: 앞면(커버)이 보이도록 설정
        Motion.transform.rotation = Quaternion.identity;
        isFlipped = false;
    }
    

    // 마우스 올렸을 때: 뒷면(정보)으로 회전
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopFlip();
        flipCoroutine = StartCoroutine(RotateCard(180f));
    }

    // 마우스 뗐을 때: 다시 앞면(커버)으로 회전
    public void OnPointerExit(PointerEventData eventData)
    {
        StopFlip();
        flipCoroutine = StartCoroutine(RotateCard(0f));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(gameObject.name + " 선택됨!");
        PlayerStats.Instance.ApplyCard(currentData);
        RewardUIManager.Instance.HideUIGameObj(RewardUIManager.Instance.pickCardGroup);
    }

    private void StopFlip()
    {
        if (flipCoroutine != null) StopCoroutine(flipCoroutine);
    }

    IEnumerator RotateCard(float targetY)
    {
        float elapsed = 0f;
        Quaternion startRot = Motion.transform.rotation;
        Quaternion endRot = Quaternion.Euler(0, targetY, 0);

        while (elapsed < flipDuration)
        {
            elapsed += Time.unscaledDeltaTime; // 일시정지 중에도 작동하도록 unscaled 사용
            float progress = elapsed / flipDuration;
            Motion.transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / flipDuration);

            float currentY = Motion.transform.rotation.eulerAngles.y;

            // eulerAngles는 0~360 사이의 값을 가짐
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