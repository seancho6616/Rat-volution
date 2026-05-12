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
    public TMP_Text waveTxt;
    public TMP_Text cheeseTxt;

    [Header("Management")]
    public StopManager stopManager;

    //정보 데이터 연결 필요

    //정보 데이터 설정 필요



    void Start()
    {
        if (GameoverGroup != null) GameoverGroup.SetActive(false);
    }

    public void SetupGameOverUI(int level, int cheese, ApiManager.Stats stats)
    {
        GameoverGroup.SetActive(true); // 게임오버 창 켜기

        // 1. 중앙 상단 텍스트 (웨이브 & 치즈)
        if (waveTxt != null)
            waveTxt.text = $"{level}";
        
        if (cheeseTxt != null)
            cheeseTxt.text = $"{cheese}";

        // 2. 좌측 텍스트 (카드별 획득 개수)
        if (cardTxt != null)
            cardTxt.text = "일반 : 0\n레어 : 0\n전설 : 0\n디버프 : 0";

        // 3. 우측 텍스트 (최종 스탯)
        if (statTxt != null)
            statTxt.text = $"이동속도 : {stats.move_speed}\n오브젝트 공격력 : {stats.attack_power}\n벽 공격력 : {stats.power}\n공격속도 : {stats.attack_speed}";
    }

    // 로비버튼 클릭 시
    public void OnLobbyButtonClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(lobbySceneName);
    }

    // 게임 종료 버튼 클릭 시
    public void OnExitButtonClicked()
    {
        if (stopManager != null)
        {
            // StopManager에게 종료 팝업을 띄워달라고 요청
            stopManager.ShowExitPopup();
        }
    }
}
