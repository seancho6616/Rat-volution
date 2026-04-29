using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class GameoverUI : MonoBehaviour
{
    [Header("Scene")]
    public string lobbySceneName = "SecondScene";

    [Header("GameOver")]
    public GameObject GameoverGroup;

    [Header("Exit")]
    public GameObject stopWindow;

    [Header("UI")]
    public TMP_Text waveCheeseTxt;
    public TMP_Text cardTxt;
    public TMP_Text statTxt;

    //정보 데이터 연결 필요

    //정보 데이터 설정 필요



    void Start()
    {
        if (GameoverGroup != null) GameoverGroup.SetActive(false);
    }

    // 로비버튼 클릭 시
    public void OnLobbyButtonClicked()
    {
        SceneManager.LoadScene(lobbySceneName);
    }

    // 게임 종료 버튼 클릭 시
    public void OnExitButtonClicked()
    {
        stopWindow.SetActive(true);
    }

    // 종료 팝업에서 [취소] 버튼을 눌렀을 때
    public void OnNoButtonClicked()
    {
        stopWindow.SetActive(false);
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
