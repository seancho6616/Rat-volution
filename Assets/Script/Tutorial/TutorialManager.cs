using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        [Header("조교 대사 (여러 줄 가능)")]
        [TextArea(2, 5)]
        public string[] dialogueLines;

        [Header("미션")]
        [Tooltip("비워두면 대사만 하고 자동으로 넘어감")]
        public string missionConditionID;
        [TextArea(1, 3)]
        public string missionHintText;   // 미션 중 화면에 띄울 안내 (예: "WASD로 움직여보세요")

        [Header("미션 완료 후 조교 반응")]
        [TextArea(2, 5)]
        public string[] successDialogueLines;
    }

    public static TutorialManager Instance { get; private set; }

    [Header("단계 목록")]
    [SerializeField] private List<TutorialStep> steps;

    [Header("조교 UI")]
    [SerializeField] private GameObject dialoguePanel;     // 조교 대사창
    [SerializeField] private TMP_Text speakerNameText;     // 조교 이름
    [SerializeField] private TMP_Text dialogueText;        // 대사 본문
    [SerializeField] private GameObject nextIndicator;     // 다음 대사 표시 (▼ 같은 아이콘)

    [Header("미션 UI")]
    [SerializeField] private GameObject missionPanel;      // 미션 중 표시되는 작은 안내창
    [SerializeField] private TMP_Text missionHintText;

    [Header("설정")]
    [SerializeField] private string instructorName;
    [SerializeField] private float typingSpeed = 0.03f;
    [SerializeField] private string mainGameSceneName = "MainGame";

    private int currentStepIndex = -1;
    private int currentLineIndex = 0;
    private bool isTyping;
    private bool waitingForMission;
    private string currentFullLine;
    private Coroutine typingCoroutine;

    // 현재 어떤 대사 묶음을 출력 중인지 (시작 대사 / 성공 대사)
    private enum DialoguePhase { Intro, Success }
    private DialoguePhase currentPhase;

    private TutorialStep CurrentStep =>
        (currentStepIndex >= 0 && currentStepIndex < steps.Count) ? steps[currentStepIndex] : null;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        speakerNameText.text = instructorName;
        missionPanel.SetActive(false);
        BeginTutorial();
    }

    private void Update()
    {
        // 미션 중일 때는 입력 무시 (플레이어가 행동해야 진행)
        if (waitingForMission) return;

        // 대사 진행: 클릭/스페이스로 다음 줄
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            OnDialogueAdvanceInput();
        }
    }

    public void BeginTutorial()
    {
        currentStepIndex = -1;
        AdvanceToNextStep();
    }

    private void AdvanceToNextStep()
    {
        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
        {
            CompleteTutorial();
            return;
        }

        // 새 단계는 항상 시작 대사부터
        currentPhase = DialoguePhase.Intro;
        currentLineIndex = 0;
        ShowDialoguePanel(true);
        PlayCurrentLine();
    }

    private void OnDialogueAdvanceInput()
    {
        // 타이핑 중이면 즉시 완성
        if (isTyping)
        {
            CompleteTypingImmediately();
            return;
        }

        // 다음 줄로
        currentLineIndex++;
        var lines = GetCurrentLines();

        if (currentLineIndex < lines.Length)
        {
            PlayCurrentLine();
        }
        else
        {
            OnDialogueBlockFinished();
        }
    }

    private string[] GetCurrentLines()
    {
        if (CurrentStep == null) return new string[0];
        return currentPhase == DialoguePhase.Intro
            ? CurrentStep.dialogueLines
            : CurrentStep.successDialogueLines;
    }

    private void PlayCurrentLine()
    {
        var lines = GetCurrentLines();
        if (lines == null || currentLineIndex >= lines.Length)
        {
            OnDialogueBlockFinished();
            return;
        }

        currentFullLine = lines[currentLineIndex];
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(currentFullLine));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        for (int i = 0; i < line.Length; i++)
        {
            dialogueText.text += line[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void CompleteTypingImmediately()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.text = currentFullLine;
        isTyping = false;
        nextIndicator.SetActive(true);
    }

    // 현재 대사 묶음(시작 또는 성공)이 모두 끝났을 때
    private void OnDialogueBlockFinished()
    {
        if (currentPhase == DialoguePhase.Intro)
        {
            // 시작 대사 끝났음 → 미션이 있으면 미션 시작, 없으면 다음 단계
            if (!string.IsNullOrWhiteSpace(CurrentStep.missionConditionID))
            {
                StartMission();
            }
            else
            {
                AdvanceToNextStep();
            }
        }
        else // Success
        {
            // 성공 대사도 끝났으면 다음 단계로
            AdvanceToNextStep();
        }
    }

    private void StartMission()
    {
        waitingForMission = true;
        ShowDialoguePanel(false);

        // 미션 힌트 표시
        if (!string.IsNullOrWhiteSpace(CurrentStep.missionHintText))
        {
            missionPanel.SetActive(true);
            missionHintText.text = CurrentStep.missionHintText;
        }
    }

    // 본 게임 코드에서 호출: 플레이어가 어떤 행동을 완료했을 때
    public void NotifyCondition(string conditionID)
    {
        if (!waitingForMission || CurrentStep == null) return;
        if (CurrentStep.missionConditionID != conditionID) return;

        OnMissionCompleted();
    }

    private void OnMissionCompleted()
    {
        waitingForMission = false;
        missionPanel.SetActive(false);

        // 성공 대사가 있으면 출력, 없으면 바로 다음 단계
        if (CurrentStep.successDialogueLines != null && CurrentStep.successDialogueLines.Length > 0)
        {
            currentPhase = DialoguePhase.Success;
            currentLineIndex = 0;
            ShowDialoguePanel(true);
            PlayCurrentLine();
        }
        else
        {
            AdvanceToNextStep();
        }
    }

    private void ShowDialoguePanel(bool show)
    {
        dialoguePanel.SetActive(show);
    }

    private void CompleteTutorial()
    {
        dialoguePanel.SetActive(false);
        missionPanel.SetActive(false);

        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();

        if (!string.IsNullOrEmpty(mainGameSceneName))
            SceneManager.LoadScene(mainGameSceneName);
    }

    public void SkipTutorial()
    {
        CompleteTutorial();
    }
}