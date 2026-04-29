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
        //if (GameoverGroup != null) GameoverGroup.SetActive(false);
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
}
