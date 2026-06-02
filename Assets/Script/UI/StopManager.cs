using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro; 
using UnityEngine.UI;

public class StopManager : MonoBehaviour
{
    [Header("버튼 연결")]
    public Button stopBtn;   // 일시정지(로비 호출) 버튼

    [Header("UI")]
    public GameObject stopGroup;
    public TMP_Text mainTxt;
    public TMP_Text warningTxt; 

    [Header("Scene")]
    public string lobbySceneName = "SecondScene";

    [Header("Audio Sources")]
    public AudioSource clickSound;

    // 현재 어떤 목적으로 창이 열렸는지 확인하기 위한 상태값
    private bool isLobbyMode = false;

    void Start()
    {
        // 게임이 처음 시작될 때는 종료 창이 보이지 않도록 비활성화
        if (stopGroup != null)
        {
            stopGroup.SetActive(false);
        }

        // 스크립트에서 버튼 클릭 이벤트 직접 할당
        if (stopBtn != null) stopBtn.onClick.AddListener(OpenLobbyPopup);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenLobbyPopup();
        }
    }

    // 우측 상단 'Stop BT'를 눌렀을 때 실행될 함수
    public void OpenLobbyPopup()
    {
        stopGroup.SetActive(true);
        isLobbyMode = true;
        if (mainTxt != null)
            mainTxt.text = "로비로 돌아가시겠습니까?";
        if (warningTxt != null)
            warningTxt.text = "※ 플레이 내용이 저장되지 않습니다.";

        
        Time.timeScale = 0f; // 게임 일시정지
    }

    
    // 게임오버 창에서 종료 클릭 시 (완전 종료용)
    public void ShowExitPopup()
    {
        if (clickSound != null) clickSound.Play();
        isLobbyMode = false;
        if (mainTxt != null)
            mainTxt.text = "게임을 종료하시겠습니까?";
        if (warningTxt != null)
            warningTxt.text = " ";
        
        stopGroup.SetActive(true);
        Time.timeScale = 0f;
    }

    // 'YES' 버튼을 눌렀을 때 실행될 함수
    public void OnClickYes()
    {
        if (clickSound != null) clickSound.Play();
        Time.timeScale = 1f;

        if (isLobbyMode)
        {
            SceneManager.LoadScene(lobbySceneName);
        }
        else
        {
            Debug.Log("게임 종료");
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }

    // 'NO' 버튼을 눌렀을 때 실행될 함수
    public void OnClickNo()
    {
        if (clickSound != null) clickSound.Play();
        stopGroup.SetActive(false); // 창 숨기기
        if (isLobbyMode)
        {
            Time.timeScale = 1f;         
        }
    }

}