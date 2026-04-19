using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardUIManager : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject rewardBTGroup; 
    public GameObject pickStatGroup;
    public GameObject pickCardGroup;

    [Header("Choose 텍스트 연결")]
    public TMP_Text txtChoose; // Hierarchy의 'Choose' 오브젝트 연결

    [Header("스탯 텍스트 연결 (버튼 내부의 Text들)")]
    public TMP_Text txtAttackPower;
    public TMP_Text txtAttackSpeed;
    public TMP_Text txtHealth;
    public TMP_Text txtInsight;
    public TMP_Text txtLuck;
    public TMP_Text txtMoveSpeed;
    public TMP_Text txtStrength;

    // 이전 레벨을 기억해 둘 변수
    private int lastLevel; 
    private bool isViewMode = false; // 현재 창이 '단순 보기' 모드인지 확인

    void Start()
    {
        // 게임 시작 시 보상 선택창(Reward BT Group) 숨김
        if (rewardBTGroup != null)
        {
            rewardBTGroup.SetActive(false);
        }

        if (pickStatGroup != null)
        {
            pickStatGroup.SetActive(false);
        }

        if (pickCardGroup != null)
        {
            pickCardGroup.SetActive(false);
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
                isViewMode = false; // 레벨업 시에는 '강화 모드'
                ShowRewardUI(); // 보상 창 열기
            }
        }
    }

    // 스탯 확인 버튼 클릭 시
    public void OpenStatView()
    {
        isViewMode = true; // '단순 보기' 모드 활성화
        UpdateStatTexts(); // 텍스트 갱신
        pickStatGroup.SetActive(true);
        Time.timeScale = 0f; // 게임 일시정지
    }

    // 실제 데이터를 UI에 반영하는 함수
    private void UpdateStatTexts()
    {
        var stats = PlayerStats.Instance;
        if (stats == null) return;

        if (isViewMode)
        {
            // 1. [확인 모드] 내 현재 능력치 상태 보기
            // PlayerStats.cs에 정의된 Final... 속성값을 가져옴
            if (txtAttackPower != null) txtAttackPower.text = $"{stats.FinalObjectAttack}";
            if (txtAttackSpeed != null) txtAttackSpeed.text = $"{stats.FinalAttackSpeed:F1}";
            if (txtHealth != null)      txtHealth.text = $"{stats.FinalMaxHP}";
            if (txtInsight != null)     txtInsight.text = $"{stats.FinalInsight}";
            if (txtLuck != null)        txtLuck.text = $"{stats.FinalLuck:F2}";
            if (txtMoveSpeed != null)   txtMoveSpeed.text = $"{stats.FinalMoveSpeed:F1}";
            if (txtStrength != null)    txtStrength.text = $"{stats.FinalWallAttack}";
        }
        else
        {
            // 2. [보상 모드] 스탯 상승량 고정 표시 (+n)
            if (txtAttackPower != null) txtAttackPower.text = "+ 2";
            if (txtAttackSpeed != null) txtAttackSpeed.text = "+ 0.1";
            if (txtHealth != null)      txtHealth.text = "+ 0.5";
            if (txtInsight != null)     txtInsight.text = "-";
            if (txtLuck != null)        txtLuck.text = "+ 0.05";
            if (txtMoveSpeed != null)   txtMoveSpeed.text = "+ 0.1";
            if (txtStrength != null)    txtStrength.text = "+ 2";
        }
    }

    // 보상 창을 여는 함수
    public void ShowRewardUI()
    {
        if (rewardBTGroup != null)
        {
            
            if (txtChoose != null) txtChoose.text = "CHOOSE"; // 초기화
            rewardBTGroup.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // 보상 선택 창
    // --- 마우스 오버 시 호출될 함수들 ---
    public void OnHoverStat()
    {
        if (txtChoose != null) txtChoose.text = "STAT";
    }

    public void OnHoverCard()
    {
        if (txtChoose != null) txtChoose.text = "CARD";
    }

    public void OnHoverExit()
    {
        if (txtChoose != null) txtChoose.text = "CHOOSE";
    }



    // 카드 선택을 골랐을 때
    public void ShowPickCardUI()
    {
        HideRewardUI();
        pickCardGroup.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ButtonCard1()
    {
        HideUIGameObj(pickCardGroup);
    }

    public void ButtonCard2()
    {
        HideUIGameObj(pickCardGroup);
    }

    public void ButtonCard3()
    {
        HideUIGameObj(pickCardGroup);
    }



    // 스탯 선택을 골랐을 때
    public void ShowPickStatUI()
    {
        isViewMode = false; // 레벨업으로 들어온 경우 '강화 모드'
        UpdateStatTexts();
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

    public void HideUIGameObj(GameObject gameObject)
    {
        if(gameObject != null)
        {
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }

// --- 버튼 클릭 함수들 (isViewMode일 때는 작동 안 하게 방어 코드 추가) ---
    public void ButtonAttackPower()
    {
        if (isViewMode) return; // 보기 모드면 클릭 무시
        PlayerStats.Instance.InvestStatPoint(StatType.ObjectAttack);
        Debug.Log(PlayerStats.Instance.runBonus.objectAttack);
        HideUIGameObj(pickStatGroup);
    }
    public void ButtonAttackSpeed()
    {
        if (isViewMode) return;
        PlayerStats.Instance.InvestStatPoint(StatType.AttackSpeed);
        Debug.Log(PlayerStats.Instance.runBonus.attackSpeed);
        HideUIGameObj(pickStatGroup);
    }
    public void ButtonHealth()
    {
        if (isViewMode) return;
        PlayerStats.Instance.InvestStatPoint(StatType.MaxHP);
        Debug.Log(PlayerStats.Instance.runBonus.maxHP);
        HideUIGameObj(pickStatGroup);
    }
    public void ButtonLuck()
    {
        if (isViewMode) return;
        PlayerStats.Instance.InvestStatPoint(StatType.Luck);
        Debug.Log(PlayerStats.Instance.runBonus.luck);
        HideUIGameObj(pickStatGroup);
    }
    public void ButtonMoveSpeed()
    {
        if (isViewMode) return;
        PlayerStats.Instance.InvestStatPoint(StatType.MoveSpeed);
        Debug.Log(StatType.MoveSpeed);
        Debug.Log(PlayerStats.Instance.runBonus.moveSpeed);
        HideUIGameObj(pickStatGroup);
    }
    public void ButtonStrength()
    {
        if (isViewMode) return;
        PlayerStats.Instance.InvestStatPoint(StatType.WallAttack);
        Debug.Log(PlayerStats.Instance.runBonus.wallAttack);
        HideUIGameObj(pickStatGroup);
    }
}