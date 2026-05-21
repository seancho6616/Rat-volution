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

    public enum StageType { InGame, Tutorial } 
    [Tooltip("튜토리얼 훈련장인지, 일반 게임 훈련장인지 선택하세요.")]
    public StageType stageType;       // 튜토리얼 / 인게임 구분용 추가
}

public class TraningSelect : MonoBehaviour
{
    [Header("Manager")]
    [Tooltip("Main2Manager 스크립트가 있는 오브젝트 추가.")]
    public Main2Manager mainManager; // 씬 이동 함수를 불러오기 위해 추가

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
    public TMP_Text warningTxt;

    [Header("Animation Settings")]
    public float animDuration = 0.3f; // 슬라이드 걸리는 시간
    public float xOffscreen = 1000f;  // 화면 밖 대기 장소의 X축 거리

    [Tooltip("가운데 이미지 크기 (기본 1)")]
    public float centerScale = 1f;
    [Tooltip("양옆 이미지 크기 (기본 0.6)")]
    public float sideScale = 0.6f;

    public Color centerColor = Color.white;
    public Color sideColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private int currentIndex = 0;
    private RectTransform[] cards = new RectTransform[3]; 
    private bool isAnimating = false; 

    // 에디터의 '위치'만 저장할 배열
    private Vector2[] slotPos = new Vector2[3];
    // 코드로 제어할 '크기'와 '색상'
    private Vector3[] slotScale = new Vector3[3];
    private Color[] slotColor = new Color[3];

    void Start()
    {
        // 1. 위치는 에디터에 배치된 값을 그대로 가져옵니다. (높이, 간격 유지)
        slotPos[0] = preparingImage1.anchoredPosition;
        slotPos[1] = trainingImage.anchoredPosition;
        slotPos[2] = preparingImage2.anchoredPosition;

        // 2. 크기와 색상은 위에서 설정한 변수값(centerScale, sideScale 등)을 적용합니다.
        slotScale[0] = new Vector3(sideScale, sideScale, 1f);
        slotScale[1] = new Vector3(centerScale, centerScale, 1f);
        slotScale[2] = new Vector3(sideScale, sideScale, 1f);

        slotColor[0] = sideColor;
        slotColor[1] = centerColor;
        slotColor[2] = sideColor;

        cards[0] = preparingImage1;
        cards[1] = trainingImage;
        cards[2] = preparingImage2;

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

        // 오른쪽 밖으로 나갈 카드의 Y축은 그대로 유지하고 X축만 멀리 보냄
        int nextIndex = (currentIndex + 1) % trainingList.Count;
        cards[2].GetComponent<Image>().sprite = trainingList[nextIndex].trainingSprite;
        cards[2].anchoredPosition = new Vector2(xOffscreen, slotPos[2].y);

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

        // 왼쪽 밖으로 나갈 카드의 Y축은 그대로 유지하고 X축만 멀리 보냄
        int prevIndex = (currentIndex - 1 + trainingList.Count) % trainingList.Count;
        cards[0].GetComponent<Image>().sprite = trainingList[prevIndex].trainingSprite;
        cards[0].anchoredPosition = new Vector2(-xOffscreen, slotPos[0].y);

        StartCoroutine(AnimateCarousel());
    }

    
    // 회전 애니메이션
    private IEnumerator AnimateCarousel()
    {
        isAnimating = true;
        float elapsedTime = 0f;

        UpdateTextAndButton();

        // 목표 위치는 시작 시 저장해둔 에디터 상의 슬롯(Left, Center, Right) 데이터
        Vector2[] targetPos = { slotPos[0], slotPos[1], slotPos[2] };
        Vector3[] targetScale = { slotScale[0], slotScale[1], slotScale[2] };
        Color[] targetColor = { slotColor[0], slotColor[1], slotColor[2] };

        Vector2[] startPos = { cards[0].anchoredPosition, cards[1].anchoredPosition, cards[2].anchoredPosition };
        Vector3[] startScale = { cards[0].localScale, cards[1].localScale, cards[2].localScale };
        Color[] startColor = { cards[0].GetComponent<Image>().color, cards[1].GetComponent<Image>().color, cards[2].GetComponent<Image>().color };

        cards[1].SetAsLastSibling(); // 가운데 카드가 맨 앞으로 오게

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

        // 에디터에 배치해둔 상태로 위치, 크기, 색상 덮어쓰기
        for (int i = 0; i < 3; i++)
        {
            cards[i].anchoredPosition = slotPos[i];
            cards[i].localScale = slotScale[i];
            cards[i].GetComponent<Image>().color = slotColor[i];
        }

        cards[1].SetAsLastSibling();
        UpdateTextAndButton();
    }

    private void UpdateTextAndButton()
    {
        if(trainingNameTxt != null) trainingNameTxt.text = trainingList[currentIndex].traninigName;

        // 기존 인스펙터에 연결된 버튼 이벤트를 초기화 (코드로 씬 이동을 완벽하게 제어하기 위함)
        if (trainingStartBtn != null) trainingStartBtn.onClick.RemoveAllListeners();

        if (trainingList[currentIndex].isReady)
        {
            trainingStartBtn.interactable = true;
            trainingStartTxt.text = "시작하기";

            if(warningTxt != null) 
                warningTxt.gameObject.SetActive(false); 

            // 준비된 훈련장이면, 타입에 맞게 씬 이동 함수를 연결해줌
            trainingStartBtn.onClick.AddListener(() => 
            {
                if (mainManager == null) 
                {
                    Debug.LogWarning("MainManager가 연결되지 않았습니다!");
                    return;
                }

                if (trainingList[currentIndex].stageType == TrainingStage.StageType.Tutorial)
                {
                    mainManager.OnTutorialStartBtn(); // 튜토리얼 씬 로드
                }
                else
                {
                    mainManager.OnTrainingStartBtn(); // 일반 게임 씬 로드
                }
            });
        }
        else
        {
            trainingStartBtn.interactable = false;
            trainingStartTxt.text = "준비 중";

            if(warningTxt != null) 
            {
                warningTxt.gameObject.SetActive(true); 
                warningTxt.text = "!!준비중!! 진입 불가!";
            }
        }    
    }
}
