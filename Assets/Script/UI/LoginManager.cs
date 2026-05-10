using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    [Header("UI 그룹")]
    public GameObject loginGroup;
    public GameObject signupGroup;
    public GameObject accountGroup;

    [Header("Login TF")]
    public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;

    [Header("Signup TF")]
    public TMP_InputField signupUsernameInput;
    public TMP_InputField signupNicknameInput;
    public TMP_InputField signupPasswordInput;

    [Header("에러 메시지")]
    public TextMeshProUGUI loginErrorText;
    public TextMeshProUGUI signupErrorText;

    [Header("계정 표시")]
    public TextMeshProUGUI accountButtonText;

    private void Start()
    {
        // 시작 시 일단 모든 그룹 숨기기 (세션 복원 결과 따라 띄울 거)
        accountGroup.SetActive(false);
        loginGroup.SetActive(false);
        signupGroup.SetActive(false);
        loginErrorText.text = "";
        signupErrorText.text = "";

        // 저장된 토큰 있으면 자동 로그인 시도
        StartCoroutine(TryAutoLogin());
    }

    // --- 자동 로그인 (세션 복원) ---
    private IEnumerator TryAutoLogin()
    {
        if (ApiManager.instance == null || !ApiManager.instance.HasToken())
        {
            // 저장된 토큰 없음 → 메인 화면
            ShowMainPanel();
            yield break;
        }

        Debug.Log("[Login] 저장된 토큰 발견 - 세션 복원 시도");

        yield return StartCoroutine(ApiManager.instance.RestoreSession(
            onSuccess: () =>
            {
                Debug.Log("[Login] 자동 로그인 성공: " + GameManager.instance.nickname);

                if (accountButtonText != null)
                {
                    accountButtonText.text = GameManager.instance.nickname;
                }
                ShowMainPanel();
            },
            onFail: (error) =>
            {
                // 토큰 만료/무효 → ApiManager가 자동으로 토큰 지움
                Debug.LogWarning("[Login] 자동 로그인 실패 - 로그인 화면 표시");
                ShowMainPanel();
            }
        ));
    }

    // --- 에러 메시지 3초 표시 코루틴 ---
    private IEnumerator ShowErrorRoutine(TextMeshProUGUI errorTextUI, string message)
    {
        errorTextUI.text = message;
        yield return new WaitForSeconds(2f);
        errorTextUI.text = "";
    }

    // --- 패널 전환 ---

    public void ShowMainPanel()
    {
        accountGroup.SetActive(true);
        loginGroup.SetActive(false);
        signupGroup.SetActive(false);
    }

    public void ShowLoginPanel()
    {
        accountGroup.SetActive(false);
        loginGroup.SetActive(true);
        signupGroup.SetActive(false);
        loginErrorText.text = "";
    }

    public void ShowSignupPanel()
    {
        accountGroup.SetActive(false);
        loginGroup.SetActive(false);
        signupGroup.SetActive(true);
        signupErrorText.text = "";
    }

    // --- 로그인 버튼 ---
    public void OnLoginButtonClicked()
    {
        string login_id = loginUsernameInput.text;
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(login_id) || string.IsNullOrEmpty(password))
        {
            StartCoroutine(ShowErrorRoutine(loginErrorText, "아이디와 비밀번호를 입력해주세요"));
            return;
        }

        StartCoroutine(LoginCoroutine(login_id, password));
    }

    private IEnumerator LoginCoroutine(string login_id, string password)
    {
        yield return StartCoroutine(ApiManager.instance.Login(
            login_id,
            password,
            onSuccess: () =>
            {
                Debug.Log("로그인 성공");
                if (accountButtonText != null)
                {
                    accountButtonText.text = login_id;
                    StartCoroutine(ShowErrorRoutine(loginErrorText, "로그인 성공!"));
                    ShowMainPanel();
                }
            },
            onFail: (error) =>
            {
                StartCoroutine(ShowErrorRoutine(loginErrorText, "! 로그인 에러 !"));
            }
        ));
    }

    // --- 회원가입 버튼 ---
    public void OnSignupButtonClicked()
    {
        string login_id = signupUsernameInput.text;
        string nickname = signupNicknameInput.text;
        string password = signupPasswordInput.text;

        if (string.IsNullOrEmpty(login_id) || string.IsNullOrEmpty(password))
        {
            StartCoroutine(ShowErrorRoutine(signupErrorText, "아이디와 비밀번호를 입력해주세요"));
            return;
        }

        if (string.IsNullOrEmpty(nickname))
        {
            StartCoroutine(ShowErrorRoutine(signupErrorText, "닉네임을 입력해주세요"));
            return;
        }

        StartCoroutine(SignupCoroutine(login_id, nickname, password));
    }

    private IEnumerator SignupCoroutine(string login_id, string nickname, string password)
    {
        yield return StartCoroutine(ApiManager.instance.Register(
            login_id,
            nickname,
            password,
            onSuccess: () =>
            {
                Debug.Log("회원가입 성공");
                StartCoroutine(ShowErrorRoutine(signupErrorText, "회원가입 성공! 로그인 해주세요"));
                ShowLoginPanel();
            },
            onFail: (error) =>
            {
                StartCoroutine(ShowErrorRoutine(signupErrorText, error));
            }
        ));
    }

    // --- 게스트 버튼 ---
    public void OnGuestButtonClicked()
    {
        StartCoroutine(GuestCoroutine());
    }

    private IEnumerator GuestCoroutine()
    {
        yield return StartCoroutine(ApiManager.instance.GuestLogin(
            onSuccess: () =>
            {
                Debug.Log("게스트 로그인 성공");
                if (accountButtonText != null)
                {
                    accountButtonText.text = "Guest";
                }
            },
            onFail: (error) =>
            {
                loginErrorText.text = error;
            }
        ));
    }

    // --- 로그아웃 (선택) ---
    // 나중에 옵션 F (logout) 작업할 때 사용
    public void OnLogoutButtonClicked()
    {
        if (ApiManager.instance != null)
        {
            ApiManager.instance.ClearToken();
        }
        if (GameManager.instance != null)
        {
            GameManager.instance.userId = "";
            GameManager.instance.nickname = "";
        }
        if (accountButtonText != null)
        {
            accountButtonText.text = "Login";
        }
        ShowLoginPanel();
        Debug.Log("[Login] 로그아웃 완료");
    }
}