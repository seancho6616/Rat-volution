using UnityEngine;
using UnityEngine.UI;

public class CardGlowController : MonoBehaviour
{
    [Header("후광 이미지 연결")]
    public Image glowImage; // 프리팹 안에 만든 Glow_Image를 여기에 연결하세요.

    [Header("등급별 색상 설정")]
    public Color normalColor = new Color(0.6f, 0.2f, 1f);   // 노말: 초록
    public Color rareColor = new Color(0.6f, 0.2f, 1f);     // 레어: 파랑
    public Color legendColor = new Color(0.6f, 0.2f, 1f);   // 레전드: 보라 (자유롭게 변경 가능)

    // CardUI에서 이 함수를 호출하여 빛을 조절합니다.
    public void SetupGlow(CardRarity rarity, bool showGlow)
    {
        if (glowImage == null) return;

        // 1. 빛 켜기/끄기
        glowImage.gameObject.SetActive(showGlow);

        // 2. 켜져있을 경우 등급에 맞는 색상 부여
        if (showGlow)
        {
            switch (rarity)
            {
                case CardRarity.Normal:
                    glowImage.color = normalColor;
                    break;
                case CardRarity.Rare:
                    glowImage.color = rareColor;
                    break;
                case CardRarity.Legend:
                    glowImage.color = legendColor;
                    break;
                default:
                    glowImage.color = Color.clear;
                    break;
            }
        }
    }
}