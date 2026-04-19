using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    [Header("UI 그룹")]
    public GameObject loginGroup;
    public GameObject signupGroup;
    public GameObject mainGroup;

    [Header("Login TF")]
    public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;

    [Header("Signup TF")]
    public TMP_InputField signupUsernameInput;
    public TMP_InputField signupNicknameInput;
    public TMP_InputField signupPasswordInput;

    [Header("에러 메시지")]
    public TextMeshProUGUI loginErrorText;      // 로그인 실패 메시지 표시
    public TextMeshProUGUI signupErrorText;     // 회원가입 실패 메시지 표시

    [Header("Scene")]
    public string nextSceneName = "SampleScene";

    private void Start()
    {
        ShowMainPanel();
    }

    // --- 패널 전환 ---

    public void ShowMainPanel()
    {
        mainGroup.SetActive(true);
        loginGroup.SetActive(false);
        signupGroup.SetActive(false);
    }

    public void ShowLoginPanel()
    {
        mainGroup.SetActive(false);
        loginGroup.SetActive(true);
        signupGroup.SetActive(false);
        loginErrorText.text = "";   // 이전 에러 메시지 초기화
    }

    public void ShowSignupPanel()
    {
        mainGroup.SetActive(false);
        loginGroup.SetActive(false);
        signupGroup.SetActive(true);
        signupErrorText.text = "";  // 이전 에러 메시지 초기화
    }

    // --- 로그인 버튼 ---
    public void OnLoginButtonClicked()
    {
        string login_id = loginUsernameInput.text;
        string password = loginPasswordInput.text;

        if (string.IsNullOrEmpty(login_id) || string.IsNullOrEmpty(password))
        {
            loginErrorText.text = "아이디와 비밀번호를 입력해주세요";
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
                SceneManager.LoadScene(nextSceneName);
            },
            onFail: (error) =>
            {
                loginErrorText.text = error;
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
            signupErrorText.text = "아이디와 비밀번호를 입력해주세요";
            return;
        }

        if (string.IsNullOrEmpty(nickname))
        {
            signupErrorText.text = "닉네임을 입력해주세요";
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
                signupErrorText.text = "회원가입 성공! 로그인 해주세요";
                ShowLoginPanel();
            },
            onFail: (error) =>
            {
                signupErrorText.text = error;
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
                SceneManager.LoadScene(nextSceneName);
            },
            onFail: (error) =>
            {
                loginErrorText.text = error;
            }
        ));
    }
}