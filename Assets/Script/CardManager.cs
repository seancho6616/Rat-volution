using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    // 플레이어 능력 연동
    public PlayerSkill playerSkill;

    [Header("카드")]
    [SerializeField]private List<CardStatData> statCards;
    [SerializeField]private List<CardItemData> itemCards;
    [SerializeField]private List<CardDebuffData> debuffCards;

    [Header("레어도 가중치")]
    [SerializeField] private int weightNormal = 50;
    [SerializeField] private int weightRare = 35;
    [SerializeField] private int weightLegend = 15;

    public List<GameObject> cardUIs; 

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LevelUP()
    {
        Debug.Log("ss");
        List<BaseCardData> pickCard = DrawCards(cardUIs.Count);
        for(int i=0; i<pickCard.Count; i++)
        {
            CardUI cardUI = cardUIs[i].GetComponent<CardUI>();
            cardUI.SetCardData(pickCard[i]);
            cardUI.frontView.SetActive(true);
            cardUI.backView.SetActive(false);
        }
        pickCard.Clear();
    }

    private List<BaseCardData> DrawCards(int count)
    {
        var allCards = new List<BaseCardData>();
        allCards.AddRange(statCards);
        allCards.AddRange(itemCards);
        allCards.AddRange(debuffCards);

        var drawn = new List<BaseCardData>();
        var pool = new List<BaseCardData>(allCards);

        for(int i=0; i<count&& pool.Count>0; i++)
        {
            CardRarity rarity = PickRarity();
            var candidates = pool.FindAll(c => c.cardRarity == rarity);
            if(candidates.Count == 0)
                candidates = pool.FindAll(c => c.cardRarity == CardRarity.Normal);
            if(candidates.Count == 0)   break;

            BaseCardData picked = candidates[Random.Range(0, candidates.Count)];
            drawn.Add(picked);
            pool.Remove(picked);
        }

        return drawn;
    }

    private CardRarity PickRarity()
    {
        int total = weightNormal + weightRare + weightLegend;
        int one = Random.Range(0, total);
        if (one < weightNormal) return CardRarity.Normal;
        if (one < weightNormal + weightRare) return CardRarity.Rare;
        return CardRarity.Legend;
    }

    // 카드 효과 적용
    public void OnCardClick(BaseCardData data)
    {
        if (data == null) return;

        if (playerSkill!= null && playerSkill.playerStats != null)
        {
            playerSkill.playerStats.ApplyCard(data);
            Debug.Log($"카드 효과 적용: {data.cardName}");
        }

        if (playerSkill != null)
        {
            playerSkill.ApplyCard(data.cardName, data.amount);
        }
        else
        {
            Debug.LogWarning("PlayerSkill 컴포넌트가 할당되지 않았습니다.");
        }
        Debug.Log($"카드 효과 적용: {data.cardName} - {data.amount}");

        HideCardSelection();
    }

    // 카드 선택창 숨기기
    public void HideCardSelection()
    {
        if (cardUIs == null || cardUIs.Count == 0) return;

        // 모든 카드 UI의 공통 부모(보상 패널)를 비활성화
        Transform parent = cardUIs[0].transform.parent;
        if (parent != null)
        {
            parent.gameObject.SetActive(false);
        }
        else
        {
            // 부모 없으면 카드 하나하나 끄기
            foreach (var card in cardUIs)
            {
                card.SetActive(false);
            }
        }
    }
}
