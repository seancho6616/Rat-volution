using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gauge : MonoBehaviour
{
    public Image gaugeImage;
    public TextMeshProUGUI countText;

    private float currentScore = 0f;
    public float maxScore = 49f;

    void Start()
    {
        UpdateUI();
    }

    public void AddScore(float amount)
    {
        currentScore += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (gaugeImage != null)
            gaugeImage.fillAmount = currentScore / maxScore;
        
        // 텍스트를 "현재 점수 / 최대 점수" 형식으로 변경
        if (countText != null)
            countText.text = $"{currentScore} / {maxScore}";
    }
}
