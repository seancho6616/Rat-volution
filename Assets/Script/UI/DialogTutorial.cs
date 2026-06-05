using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class DialogTutorial : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText;

    [Header("튜토리얼 내용")]
    [TextArea(3, 5)]
    public string[] sentences;

    [Header("타이핑 설정")]
    public float typingSpeed = 0.05f;

    [Header("대사별 오브젝트 이벤트")]
    public TutorialObjectEvent[] objectEvents;

    private int currentIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        if (sentences.Length > 0)
        {
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f;
            ApplyObjectEvent(0);
            typingCoroutine = StartCoroutine(TypeSentence(sentences[0]));
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (tutorialPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) ||  Input.GetKeyDown(KeyCode.Space))
                DisplayNextSentence();
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        tutorialText.text = "";
        foreach (char letter in sentence)
        {
            tutorialText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isTyping = false;
    }

    public void DisplayNextSentence()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            tutorialText.text = sentences[currentIndex];
            isTyping = false;
            return;
        }

        currentIndex++;

        if (currentIndex < sentences.Length)
        {
            ApplyObjectEvent(currentIndex);
            typingCoroutine = StartCoroutine(TypeSentence(sentences[currentIndex]));
        }
        else
        {
            EndTutorial();
        }
    }

    void ApplyObjectEvent(int index)
    {
        if (objectEvents == null || index >= objectEvents.Length) return;

        TutorialObjectEvent e = objectEvents[index];

        foreach (var obj in e.objectsToShow)
            if (obj != null) obj.SetActive(true);

        foreach (var obj in e.objectsToHide)
            if (obj != null) obj.SetActive(false);
    }

    void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("SecondScene");
    }
}

[System.Serializable]
public class TutorialObjectEvent
{
    public GameObject[] objectsToShow;
    public GameObject[] objectsToHide;
}