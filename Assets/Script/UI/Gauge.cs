using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gauge : MonoBehaviour
{
    public Image gaugeImage;
    public TextMeshProUGUI countText;

    void Start()
    {
        UpdateUI();
    }

    public void AddScore(float amount)
    {
        UpdateUI();
    }

    void UpdateUI()
    {
       // PlayerStats의 싱글톤 인스턴스가 있는지 확인 후 데이터 가져옴
        if (PlayerStats.Instance != null)
        {
            float current = PlayerStats.Instance.currentCheese;
            float max = PlayerStats.Instance.maxCheese;

            // 게이지 바 채우기
            if (gaugeImage != null)
            {
                gaugeImage.fillAmount = current / max;
            }
            
            // 텍스트 업데이트
            if (countText != null)
            {
                countText.text = $"{current} / {max}";
            }
        }
    }
}