using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class Main2Manager : MonoBehaviour
{
    [Header("Scene")]
    public string homeSceneName = "FirstScene";
    public string inGameScene = "SampleScene";

    [Header("Hide")]
    public GameObject inventory;

    [Header("Setting")]
    public GameObject settingWindow;
    public GameObject settingGroup;

    [Header("Training Select")]
    public GameObject selectWindow;
    public GameObject trainingSelectGroup;

    private void Start()
    {
        if (settingWindow != null) settingWindow.gameObject.SetActive(false);
        if (selectWindow != null) selectWindow.gameObject.SetActive(false);
    }

    // 세팅 버튼을 눌렀을 때
    public void OnSettingBtn()
    {
        inventory.SetActive(false);
        settingWindow.SetActive(true);
    }

    // 세팅에서 나가기 버튼 클릭 시
    public void OnSettingExitBtn()
    {
        settingWindow.SetActive(false);
        inventory.SetActive(true);
    }

    // 홈 버튼을 눌렀을 때
    public void OnHomeBtn()
    {
        SceneManager.LoadScene(homeSceneName);
    }

    // 훈련장 선택 버튼 눌렀을 때 
    public void OnTrainingSelectBtn()
    {
        inventory.SetActive(false);
        settingGroup.SetActive(false);
        selectWindow.SetActive(true);
    }

    // 훈련장에서 뒤로가기 버튼 눌렀을 때
    public void OnTrainingBackBtn()
    {
        selectWindow.SetActive(false);
    }

    // 훈련 시작하기 버튼 눌렀을 때
    public void OnTrainingStartBtn()
    {
        SceneManager.LoadScene(inGameScene);
    }
}
