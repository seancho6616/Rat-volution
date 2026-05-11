using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Main2Manager : MonoBehaviour
{
    [Header("Scene")]
    public string homeSceneName = "FirstScene";
    public string inGameScene = "SampleScene";

    [Header("Hide Panel")]
    public GameObject inventory;
    public GameObject ranking;

    [Header("Setting")]
    public GameObject settingWindow;
    public GameObject settingGroup;
    public Slider soundSlider;
    public TMP_Text soundValueTxt;
    public TMP_Dropdown resolutionDropdown;

    [Header("Default Settings")]
    public float defaultVolume = 0.5f; // 처음 시작할 때의 기본 소리 크기 

    [Header("Training Select")]
    public GameObject selectWindow;
    public GameObject trainingSelectGroup;

    // 해상도 옵션
    private List<string> resOptions = new List<string> { "1920 x 1080", "1600 x 900", "1280 x 720" };

    private void Start()
    {
        if (settingWindow != null) settingWindow.gameObject.SetActive(false);
        if (selectWindow != null) selectWindow.gameObject.SetActive(false);

        // --- 설정창 부품 초기화 ---
        if (soundSlider != null)
        {
            soundSlider.value = defaultVolume;
            AudioListener.volume = defaultVolume;

            // 처음 텍스트 설정 (0.0~1.0 값을 0~100으로 변환)
            if (soundSlider != null)
            {
                soundValueTxt.text = Mathf.RoundToInt(defaultVolume * 100).ToString();
            }

            // 슬라이더를 움직일 때마다 SetVolume 함수 실행
            soundSlider.onValueChanged.AddListener(SetVolume);
        }

        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(resOptions);
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }
    }

    // 세팅 버튼을 눌렀을 때
    public void OnSettingBtn()
    {
        inventory.SetActive(false);
        ranking.SetActive(false);
        settingWindow.SetActive(true);
    }

    // 세팅에서 나가기 버튼 클릭 시
    public void OnSettingExitBtn()
    {
        settingWindow.SetActive(false);
        ranking.SetActive(true);
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
        ranking.SetActive(false);
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

    // 사운드 조절
    public void SetVolume(float value)
    {
        // 유니티 전체 마스터 볼륨 조절
        AudioListener.volume = value;

        if (soundValueTxt != null)
        {
            soundValueTxt.text = Mathf.RoundToInt(value * 100).ToString();
        }
    }

    // 해상도 조절
    public void SetResolution(int index)
    {
        // 해상도 및 창모드 조절
        switch (index)
        {
            case 0: Screen.SetResolution(1920, 1080, FullScreenMode.Windowed); break;
            case 1: Screen.SetResolution(1600, 900, FullScreenMode.Windowed); break;
            case 2: Screen.SetResolution(1280, 720, FullScreenMode.Windowed); break;
        }
    }
}
