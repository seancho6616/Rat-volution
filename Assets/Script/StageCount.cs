using UnityEngine;
using TMPro;

public class StageCount : MonoBehaviour
{
    public TextMeshProUGUI stageText;

    void Start()
    {
        UpdateStageUI();
    }

    // 아이템을 먹었을 때 호출될 함수
    // Item.cs에서 아이템을 먹었을 때 호출됨
    public void OnItemCollected()
    {
        UpdateStageUI();
    }

    public void UpdateStageUI()
    {
        if (PlayerStats.Instance != null && stageText != null)
        {
            stageText.text = "STAGE : " + PlayerStats.Instance.level;
        }
    }
}
   