using UnityEngine;
using TMPro;

public class StageCount : MonoBehaviour
{
    public TextMeshProUGUI stageText;
    private int currentStage = 0;
    private int itemsRemaining; // 맵에 남은 아이템 개수를 저장할 변수

    void Start()
    {
        UpdateStageUI();
        ResetItemCount(); // 게임 시작 시 아이템 개수 파악
    }

    // 맵에 있는 아이템 개수를 새로 세는 함수
    public void ResetItemCount()
    {
        // "Item" 태그를 가진 모든 오브젝트의 개수를 가져옵니다.
        itemsRemaining = GameObject.FindGameObjectsWithTag("Item").Length;
    }

    // 아이템을 먹었을 때 호출될 함수
    public void OnItemCollected()
    {
        itemsRemaining--; // 개수를 하나 줄임

        // 만약 남은 아이템이 0개라면 스테이지 업!
        if (itemsRemaining <= 0)
        {
            NextStage();
        }
    }

    public void NextStage()
    {
        currentStage++;
        UpdateStageUI();
        
        // [중요] 다음 스테이지 아이템들이 생성된 후, 다시 개수를 세어줘야 합니다.
        // Invoke를 이용해 약간의 시간차를 두고 호출하거나, 아이템 생성 직후 호출하세요.
        Invoke("ResetItemCount", 0.1f); 
    }

    void UpdateStageUI()
    {
        if (stageText != null) stageText.text = "STAGE : " + currentStage;
    }
}