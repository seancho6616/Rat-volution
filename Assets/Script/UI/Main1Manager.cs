using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Main1Manager : MonoBehaviour
{
    [Header("Scene")]
    public string nextSceneName = "SecondScene";

    [Header("Fade Effect")]
    public Image fadeImage;             // 화면 전체를 덮을 검은색 이미지
    public RectTransform uiContainer;   // 다가오면서 커질 UI 전체 그룹 (Main Group 등)
    public float fadeDuration = 1.2f;   // 연출에 걸리는 시간
    public float zoomTargetScale = 20f; // 얼마나 크게 줌인할 것인지

    [Header("Hide Group")]
    public GameObject accountGroup;
    public GameObject languageGroup;
    public GameObject titleGroup;

    [Header("Exit")]
    public GameObject popupGroup;
    public GameObject exitGroup;

    [Header("계정 표시 (선택)")]
    public TextMeshProUGUI hiUserTxt;   // 게스트 로그인 시 닉네임 표시용

    private bool isEntering = false;   // 중복 클릭 방지

    private void Start()
    {
        // 시작할 때 페이드 이미지, 팝업 off
        if (fadeImage != null) fadeImage.gameObject.SetActive(false);
        if (popupGroup != null) popupGroup.SetActive(false);
    }

    // --- 씬 전환 및 페이드 효과 ---

    // Entry BT을 눌렀을 때 호출될 함수 (REQ-002, REQ-006)
    public void OnEntryButtonClicked()
    {
        // 중복 클릭 방지
        if (isEntering) return;

        // 이미 로그인됨 (일반 로그인 or 자동 로그인 토큰 살아있음)
        if (ApiManager.instance != null && ApiManager.instance.HasToken())
        {
            Debug.Log("[Main1] 로그인 상태 - 바로 진입");
            StartEntryFlow();
            return;
        }

        // 미로그인 → 게스트 자동 로그인
        Debug.Log("[Main1] 미로그인 상태 - 게스트 자동 로그인");
        StartCoroutine(GuestLoginAndEnter());
    }

    private IEnumerator GuestLoginAndEnter()
    {
        isEntering = true;

        yield return StartCoroutine(ApiManager.instance.GuestLogin(
            onSuccess: () =>
            {
                Debug.Log("[Main1] 게스트 로그인 성공: " + GameManager.instance.nickname);

                // 닉네임 표시(있으면)
                if (hiUserTxt != null)
                {
                    hiUserTxt.text = GameManager.instance.nickname;
                }

                StartEntryFlow();
            },
            onFail: (error) =>
            {
                Debug.LogError("[Main1] 게스트 로그인 실패: " + error);
                isEntering = false;
                // 서버 연결 실패 시 진입 차단 (사용자 알림은 추후 추가)
            }
        ));
    }

    // 페이드 + 씬 전환 시작
    private void StartEntryFlow()
    {
        isEntering = true;

        if (accountGroup != null) accountGroup.SetActive(false);
        if (languageGroup != null) languageGroup.SetActive(false);
        if (titleGroup != null) titleGroup.SetActive(false);

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

            float progress = timer / fadeDuration;
            float easeIn = progress * progress * progress;

            if (fadeImage != null)
            {
                fadeColor.a = easeIn;
                fadeImage.color = fadeColor;
            }

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

    public void OnBackgroundClicked()
    {
        popupGroup.SetActive(true);
    }

    public void OnNoButtonClicked()
    {
        popupGroup.SetActive(false);
    }

    public void OnYesButtonClicked()
    {
        Debug.Log("게임을 종료합니다.");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}