using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // 씬 전환을 위한 네임스페이스

public class LoginManager : MonoBehaviour
{
    [Header("UI 그룹")]
    public GameObject loginGroup;  // 로그인 그룹 패널
    public GameObject signupGroup; // 회원가입 그룹 패널
    public GameObject mainGroup; // 메인 그룹 패널

    [Header("Login TF")]
    public TMP_InputField loginUsernameInput; // 로그인 창의 Username 입력 칸
    public TMP_InputField loginPasswordInput; // 로그인 창의 Password 입력 칸

    [Header("Scene")]
    public string nextSceneName = "SampleScene"; // 클릭 시 넘어갈 씬의 이름

    // 나중에 DB 연동 전까지 사용할 임시 계정 정보
    private readonly string dummyUsername = "admin";
    private readonly string dummyPassword = "1234";

    private void Start()
    {
        // 게임 시작 시 메인창만 활성화
        ShowMainPanel();
    }

    // --- 1. 패널 전환 기능 ---

    public void ShowMainPanel()
    {
        mainGroup.SetActive(true);
        loginGroup.SetActive(false);
        signupGroup.SetActive(false);
    }
    
    // 회원가입 버튼(Signup BT)을 눌렀을 때 실행될 함수
    public void ShowSignupPanel()
    {
        loginGroup.SetActive(false);
        signupGroup.SetActive(true);
    }

    // 다시 로그인 창으로 돌아가고 싶을 때 사용할 함수
    public void ShowLoginPanel()
    {
        loginGroup.SetActive(true);
        signupGroup.SetActive(false);
    }

    // --- 2. 로그인 및 씬 전환 기능 ---

    // 로그인 버튼(Login BT)을 눌렀을 때 실행될 함수
    public void OnLoginButtonClicked()
    {
        // string inputUsername = loginUsernameInput.text;
        // string inputPassword = loginPasswordInput.text;

        // // 입력받은 데이터와 임시 데이터를 비교
        // if (inputUsername == dummyUsername && inputPassword == dummyPassword)
        // {
        //     Debug.Log("로그인 성공! 다음 씬으로 넘어갑니다.");
            
        //     // 다음 씬으로 전환 (Build Settings에 씬이 등록되어 있어야 함)
        //     SceneManager.LoadScene(nextSceneName);
        // }
        // else
        // {
        //     Debug.Log("로그인 실패: 유저 이름이나 비밀번호가 틀렸습니다.");
        //     // UI에 오류 메시지를 띄우려면 여기에 코드를 추가하면 됩니다.
        // }
    }
}