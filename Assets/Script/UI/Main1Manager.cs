using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class Main1Manager : MonoBehaviour
{
    [Header("Scene")]
    public string nextSceneName = "SampleScene";

    [Header("Fade Effect")]
    public Image fadeImage;             // 화면 전체를 덮을 검은색 이미지
    public RectTransform uiContainer;   // 다가오면서 커질 UI 전체 그룹 (Main Group 등)
    public float fadeDuration = 1.2f;   // 연출에 걸리는 시간
    public float zoomTargetScale = 20f; // 얼마나 크게 줌인할 것인지 (수치가 클수록 확 다가옵니다)

    [Header("Hide Group")]
    public GameObject accountGroup;
    public GameObject languageGroup;
    public GameObject titleGroup;

    [Header("Exit")]
    public GameObject popupGroup;
    public GameObject exitGroup;

    // [Header("Exit Popup")]
    // public GameObject exitPopup;     // 게임 종료 확인 팝업창

    

    private void Start()
    {
        // 시작할 때 페이드 이미지, 팝업 off
        if (fadeImage != null) fadeImage.gameObject.SetActive(false);
        if (popupGroup != null) popupGroup.SetActive(false);
        
    }

    // --- 씬 전환 및 페이드 효과 ---

    // Entry BT을 눌렀을 때 호출될 함수
    public void OnEntryButtonClicked()
    {
        accountGroup.SetActive(false);
        languageGroup.SetActive(false);
        titleGroup.SetActive(false);
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        Color fadeColor = Color.black;

        // 1. 페이드 이미지 준비
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            fadeColor = fadeImage.color;
            fadeColor.a = 0f;
            fadeImage.color = fadeColor;
        }

        // 2. UI 원래 크기 기억
        Vector3 originalScale = Vector3.one;
        if (uiContainer != null)
        {
            originalScale = uiContainer.localScale;
        }

        float timer = 0f;

        // 3. 연출 시작
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            
            // 진행도 (0.0 ~ 1.0)
            float progress = timer / fadeDuration;

            // 점점 가속도가 붙으며 쑥 빨려 들어가는 느낌을 위해 세제곱 (Ease-In)
            float easeIn = progress * progress * progress; 

            // 화면을 점점 검게 (마지막에 확 까매지도록)
            if (fadeImage != null)
            {
                fadeColor.a = easeIn;
                fadeImage.color = fadeColor;
            }

            // UI를 카메라 쪽으로 크게 줌인 (블랙홀 안으로 들어가는 듯한 효과)
            if (uiContainer != null)
            {
                uiContainer.localScale = Vector3.Lerp(originalScale, originalScale * zoomTargetScale, easeIn);
            }

            yield return null;
        }

        // 4. 완전히 다 빨려 들어가면 씬 이동
        SceneManager.LoadScene(nextSceneName);
    }

    // --- 게임 종료 안내 팝업 ---

    // 화면의 빈 배경을 클릭했을 때 호출될 함수
    public void OnBackgroundClicked()
    {
        // accountGroup이나 languageGroup이 활성화되어 있다면 배경 클릭 무시
        // if (accountGroup.activeSelf || languageGroup.activeSelf) 
        // {
        //     return;
        // }

        popupGroup.SetActive(true);
    }

    // 종료 팝업에서 [취소] 버튼을 눌렀을 때
    public void OnNoButtonClicked()
    {
        popupGroup.SetActive(false);
    }

    // 종료 팝업에서 [확인/종료] 버튼을 눌렀을 때
    public void OnYesButtonClicked()
    {
        Debug.Log("게임을 종료합니다.");
        Application.Quit(); // 실제 빌드된 게임에서 종료됨

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 플레이 중지
#endif
    }
}

