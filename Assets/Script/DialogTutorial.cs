using UnityEngine;
using TMPro; 

public class DialogTutorial : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject tutorialPanel;        // 튜토리얼 전체 창 (Panel)
    public TextMeshProUGUI tutorialText;    // 글자가 들어갈 텍스트 (TMP)

    [Header("튜토리얼 내용")]
    [TextArea(3, 5)] // Inspector 창에서 여러 줄을 편하게 입력하게 해주는 속성
    public string[] sentences;              // 설명할 텍스트들을 순서대로 넣을 배열
    
    private int currentIndex = 0;           // 현재 몇 번째 텍스트인지 기억하는 변수

    void Start()
    {
        // 게임 시작 시 대사가 하나라도 등록되어 있다면 튜토리얼 시작
        if (sentences.Length > 0)
        {
            tutorialPanel.SetActive(true);          // 창 띄우기
            tutorialText.text = sentences[0];       // 첫 번째 대사 넣기
            Time.timeScale = 0f;                    // 튜토리얼 읽는 동안 게임 정지
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
    }

    void Update()
    {
        // 튜토리얼 창이 켜져 있을 때만 키보드 입력 받음
        if (tutorialPanel.activeSelf)
        {
            // 기본 엔터키(Return) 또는 숫자 패드의 엔터키(KeypadEnter)를 눌렀을 때
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                DisplayNextSentence();
            }
        }
    }

    // 버튼을 누르거나 화면을 클릭할 때마다 실행될 함수
    public void DisplayNextSentence()
    {
        currentIndex++; // 다음 대사 인덱스로 넘어감

        // 아직 보여줄 대사가 남아있다면
        if (currentIndex < sentences.Length)
        {
            tutorialText.text = sentences[currentIndex];
        }
        else // 준비된 대사를 모두 보여주었다면
        {
            EndTutorial();
        }
    }

    // 튜토리얼 종료 함수
    public void EndTutorial()
    {
        tutorialPanel.SetActive(false); // 창 숨기기
        Time.timeScale = 1f;            // 게임 다시 진행
    }
}