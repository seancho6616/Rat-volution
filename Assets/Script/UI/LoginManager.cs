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
    public TMP_Text hiUserTxt;

    [Header("로그인 상태별 버튼")]
    public GameObject accountButton;   // 비로그인 상태에서 표시
    public GameObject logoutButton;    // 로그인 상태에서 표시

    private void Start()
    {
        accountGroup.SetActive(false);
        loginGroup.SetActive(false);
        signupGroup.SetActive(false);
        loginErrorText.text = "";
        signupErrorText.text = "";

        StartCoroutine(TryAutoLogin());
    }

    // 로그인 상태에 따라 account/logout 버튼 표시 교체
    private void UpdateLoginStateUI()
    {
        bool isLoggedIn = ApiManager.instance != null && ApiManager.instance.HasToken();

        if (accountButton != null) accountButton.SetActive(!isLoggedIn);
        if (logoutButton != null) logoutButton.SetActive(isLoggedIn);
    }

    // --- 자동 로그인 (세션 복원) ---
    private IEnumerator TryAutoLogin()
    {
        if (ApiManager.instance == null || !ApiManager.instance.HasToken())
        {
            ShowMainPanel();
            yield break;
        }

        Debug.Log("[Login] 저장된 토큰 발견 - 세션 복원 시도");

        yield return StartCoroutine(ApiManager.instance.RestoreSession(
            onSuccess: () =>
            {
                Debug.Log("[Login] 자동 로그인 성공: " + GameManager.instance.nickname);

                if (hiUserTxt != null)
                {
                    hiUserTxt.text = $"Hi, {GameManager.instance.nickname}";
                }
                ShowMainPanel();
            },
            onFail: (error) =>
            {
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

        UpdateLoginStateUI();   // 메인 화면 표시할 때마다 버튼 상태 갱신
    }

    public void ShowLoginPanel()
    {
        if (ApiManager.instance != null && ApiManager.instance.HasToken())
        {
            Debug.Log("[Login] 이미 로그인된 상태 - 먼저 로그아웃 필요");
            StartCoroutine(ShowErrorRoutine(loginErrorText, "이미 로그인 되어있습니다. 로그아웃 후 시도하세요"));
            return;
        }

        accountGroup.SetActive(false);
        loginGroup.SetActive(true);
        signupGroup.SetActive(false);
        loginErrorText.text = "";
    }

    public void ShowSignupPanel()
    {
        if (ApiManager.instance != null && ApiManager.instance.HasToken())
        {
            Debug.Log("[Login] 이미 로그인된 상태 - 회원가입 불가");
            return;
        }

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

        loginErrorText.text = "로그인 중...";
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

                if (hiUserTxt != null)
                {
                    hiUserTxt.text = $"Hi, {GameManager.instance.nickname}";
                }
                StartCoroutine(ShowErrorRoutine(loginErrorText, "로그인 성공!"));
                ShowMainPanel();   // 내부에서 UpdateLoginStateUI() 호출됨
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

        signupErrorText.text = "가입 처리 중...";
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
                if (hiUserTxt != null)
                {
                    hiUserTxt.text = "Hi, Guest!";
                }
                UpdateLoginStateUI();   // 게스트 로그인 후 버튼 상태 갱신
            },
            onFail: (error) =>
            {
                loginErrorText.text = error;
            }
        ));
    }

    // --- 로그아웃 ---
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

        if (hiUserTxt != null)
        {
            hiUserTxt.text = "Hi, User!";
        }

        ShowMainPanel();   // 내부에서 UpdateLoginStateUI() 호출됨 (logout 숨김, account 표시)
        Debug.Log("[Login] 로그아웃 완료");
    }
}