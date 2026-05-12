using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

// 훈련장 각각의 데이터를 담을 클래스
[System.Serializable]
public class TrainingStage
{
    public string traninigName;       // 훈련장 이름
    public Sprite trainingSprite;     // 훈련장 이미지
    public bool isReady;           // 오픈 여부 (false면 '준비 중')
}

public class TraningSelect : MonoBehaviour
{
    [Header("Training Data")]
    public List<TrainingStage> trainingList;

    [Header("UI RectTransforms")]
    public RectTransform preparingImage1;
    public RectTransform trainingImage;
    public RectTransform preparingImage2;
    
    [Header("Txt & Btn")]
    public TMP_Text trainingNameTxt;
    public Button trainingStartBtn;
    public TMP_Text trainingStartTxt;

    [Header("Animation Settings")]
    public float animDuration = 0.3f; // 슬라이드 걸리는 시간
    public float xOffset = 550f;      // 양옆 이미지의 X축 거리
    public float xOffscreen = 1100f;  // 화면 밖 대기 장소의 X축 거리
    public Vector3 centerScale = Vector3.one;
    public Vector3 sideScale = new Vector3(0.7f, 0.7f, 1f); // 양옆 이미지 크기
    public Color centerColor = Color.white;
    public Color sideColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 양옆 이미지 색상(어둡게)

    private int currentIndex = 0;
    private RectTransform[] cards = new RectTransform[3]; // 0:Left, 1:Center, 2:Right
    private bool isAnimating = false; // 애니메이션 도중 중복 클릭 방지

    void Start()
    {
        // 3개의 슬롯을 배열에 할당
        cards[0] = preparingImage1;
        cards[1] = trainingImage;
        cards[2] = preparingImage2;

        // 초기 위치 및 스프라이트 세팅
        InitCarousel();
    }

    // 오른쪽 버튼 클릭 시
    public void OnClickRightArrow()
    {
        if (isAnimating || trainingList.Count == 0) return;
        
        currentIndex = (currentIndex + 1) % trainingList.Count;

        // 역할 교대 (컨베이어 벨트)
        RectTransform oldLeft = cards[0];
        RectTransform oldCenter = cards[1];
        RectTransform oldRight = cards[2];

        cards[0] = oldCenter; // 가운데 있던 애가 왼쪽으로
        cards[1] = oldRight;  // 오른쪽에 있던 애가 가운데로
        cards[2] = oldLeft;   // 왼쪽에 있던 애는 오른쪽(화면 밖)으로 순간이동

        // 오른쪽 밖으로 순간이동할 카드에 다음다음 이미지 미리 세팅
        int nextIndex = (currentIndex + 1) % trainingList.Count;
        cards[2].GetComponent<Image>().sprite = trainingList[nextIndex].trainingSprite;
        cards[2].anchoredPosition = new Vector2(xOffscreen, 0);

        StartCoroutine(AnimateCarousel());
    }

    // 왼쪽 버튼 클릭 시
    public void OnClickLeftArrow()
    {
        if (isAnimating || trainingList.Count == 0) return;
        
        currentIndex = (currentIndex - 1 + trainingList.Count) % trainingList.Count;

        // 역할 교대 (컨베이어 벨트 반대 방향)
        RectTransform oldLeft = cards[0];
        RectTransform oldCenter = cards[1];
        RectTransform oldRight = cards[2];

        cards[0] = oldRight; // 오른쪽에 있던 애가 왼쪽(화면 밖)으로 순간이동
        cards[1] = oldLeft;  // 왼쪽에 있던 애가 가운데로
        cards[2] = oldCenter;// 가운데 있던 애가 오른쪽으로

        // 왼쪽 밖으로 순간이동할 카드에 이전이전 이미지 미리 세팅
        int prevIndex = (currentIndex - 1 + trainingList.Count) % trainingList.Count;
        cards[0].GetComponent<Image>().sprite = trainingList[prevIndex].trainingSprite;
        cards[0].anchoredPosition = new Vector2(-xOffscreen, 0);

        StartCoroutine(AnimateCarousel());
    }

    
    // 회전 애니메이션
    private IEnumerator AnimateCarousel()
    {
        isAnimating = true;
        float elapsedTime = 0f;

        UpdateTextAndButton();

        Vector2[] targetPos = { new Vector2(-xOffset, 0), Vector2.zero, new Vector2(xOffset, 0) };
        Vector3[] targetScale = { sideScale, centerScale, sideScale };
        Color[] targetColor = { sideColor, centerColor, sideColor };

        Vector2[] startPos = { cards[0].anchoredPosition, cards[1].anchoredPosition, cards[2].anchoredPosition };
        Vector3[] startScale = { cards[0].localScale, cards[1].localScale, cards[2].localScale };
        Color[] startColor = { cards[0].GetComponent<Image>().color, cards[1].GetComponent<Image>().color, cards[2].GetComponent<Image>().color };

        cards[1].SetAsLastSibling();

        while (elapsedTime < animDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animDuration);
            float curveT = t * t * (3f - 2f * t);

            for (int i = 0; i < 3; i++)
            {
                cards[i].anchoredPosition = Vector2.Lerp(startPos[i], targetPos[i], curveT);
                cards[i].localScale = Vector3.Lerp(startScale[i], targetScale[i], curveT);
                cards[i].GetComponent<Image>().color = Color.Lerp(startColor[i], targetColor[i], curveT);
            }
            yield return null;
        }

        for (int i = 0; i < 3; i++)
        {
            cards[i].anchoredPosition = targetPos[i];
            cards[i].localScale = targetScale[i];
            cards[i].GetComponent<Image>().color = targetColor[i];
        }

        isAnimating = false;
    }

    private void InitCarousel()
    {
        if (trainingList.Count == 0) return;

        int prevIndex = (currentIndex - 1 + trainingList.Count) % trainingList.Count;
        int nextIndex = (currentIndex + 1) % trainingList.Count;

        cards[0].GetComponent<Image>().sprite = trainingList[prevIndex].trainingSprite;
        cards[1].GetComponent<Image>().sprite = trainingList[currentIndex].trainingSprite;
        cards[2].GetComponent<Image>().sprite = trainingList[nextIndex].trainingSprite;

        cards[0].anchoredPosition = new Vector2(-xOffset, 0);
        cards[1].anchoredPosition = Vector2.zero;
        cards[2].anchoredPosition = new Vector2(xOffset, 0);

        cards[0].localScale = sideScale;
        cards[1].localScale = centerScale;
        cards[2].localScale = sideScale;

        cards[0].GetComponent<Image>().color = sideColor;
        cards[1].GetComponent<Image>().color = centerColor;
        cards[2].GetComponent<Image>().color = sideColor;

        cards[1].SetAsLastSibling();
        UpdateTextAndButton();
    }

    private void UpdateTextAndButton()
    {
        if(trainingNameTxt != null) trainingNameTxt.text = trainingList[currentIndex].traninigName;

        if (trainingList[currentIndex].isReady)
        {
            trainingStartBtn.interactable = true;
            trainingStartTxt.text = "훈련 시작하기";
        }
        else
        {
            trainingStartBtn.interactable = false;
            trainingStartTxt.text = "준비 중";
        }
    }
}
