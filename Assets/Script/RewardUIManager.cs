using UnityEngine;

public class RewardUIManager : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject rewardBTGroup; 
    public GameObject pickStatGroup;

    // 이전 레벨을 기억해 둘 변수
    private int lastLevel; 

    void Start()
    {
        // 게임 시작 시 보상 선택창(Reward BT Group) 숨김
        if (rewardBTGroup != null)
        {
            rewardBTGroup.SetActive(false);
        }

        // 게임 시작 시점의 PlayerStats 레벨을 기억
        if (PlayerStats.Instance != null)
        {
            lastLevel = PlayerStats.Instance.level;
        }
    }

    void Update()
    {
        // 매 프레임마다 레벨이 올랐는지 확인
        if (PlayerStats.Instance != null)
        {
            // 현재 레벨이 기억해둔 레벨(lastLevel)보다 높아졌다면? (스테이지 1 증가)
            if (PlayerStats.Instance.level > lastLevel)
            {
                lastLevel = PlayerStats.Instance.level; // 다음 레벨업을 위해 기억 갱신
                ShowRewardUI(); // 보상 창 열기
            }
        }
    }

    // 보상 창을 여는 함수
    public void ShowRewardUI()
    {
        if (rewardBTGroup != null)
        {
            rewardBTGroup.SetActive(true); // Reward BT Group 활성화
            
            // 게임 일시정지
            Time.timeScale = 0f; 
        }
    }

    public void ShowPickStatUI()
    {
        HideRewardUI();
        pickStatGroup.SetActive(true);

        //Time.timeScale = 0f;
    }

    // 보상 창을 닫는 함수 (나중에 Card Button이나 Stat Button을 눌렀을 때 호출되도록 연결)
    public void HideRewardUI()
    {
        if (rewardBTGroup != null)
        {
            rewardBTGroup.SetActive(false); // Reward BT Group 비활성화
            
            //게임 일시정지 해제
            //Time.timeScale = 1f;
        }
    }
}