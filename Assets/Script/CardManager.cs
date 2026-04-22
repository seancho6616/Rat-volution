using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
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
        List<BaseCardData> pickCard = DrawCards(cardUIs.Count);
        for(int i=0; i<pickCard.Count; i++)
        {
            CardUI cardUI = cardUIs[i].GetComponent<CardUI>();
            cardUI.SetCardData(pickCard[i]);
        }
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
        if (one < weightRare + weightRare) return CardRarity.Rare;
        return CardRarity.Legend;
    }
}
