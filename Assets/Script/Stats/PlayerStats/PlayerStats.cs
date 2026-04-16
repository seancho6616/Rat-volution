using UnityEngine;

public enum StatType
{
    MaxHP, WallCount, Luck, Insight,
    MoveSpeed, ObjectAttack, AttackSpeed, WallAttack,
}

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    public PlayerBaseStatsData baseData;
    public RunBonus runBonus = new RunBonus();

    public int FinalMaxHP =>baseData.maxHP + runBonus.maxHP;
    public int FinalWallCount => baseData.wallCount + runBonus.wallCount;
    public float FinalMoveSpeed => baseData.moveSpeed + runBonus.moveSpeed;
    public float FinalAttackSpeed => baseData.attackSpeed + runBonus.attackSpeed;
    public float FinalObjectAttack => baseData.objectAttack + runBonus.objectAttack;
    public float FinalWallAttack => baseData.wallAttack + runBonus.wallAttack;
    public float FinalLuck => baseData.luck + runBonus.luck;
    public float FinalInsight => baseData.insight + runBonus.insight;

    [Header("치즈 개수")]
    public float maxCheese = 49;
    public float currentCheese =0;
    public int level = 1;
    private int hpCount =0;

    void Awake()
    {
        if(Instance == null) Instance = this;
        //else Destroy(gameObject);
        runBonus.Reset();
    }
    public void GainCheese(float amount)
    {
        currentCheese += amount;
        if (currentCheese >= maxCheese)
            LevelUP();
    }

    public void LevelUP()
    {
        level++;
        currentCheese = 0;
        maxCheese += 5;
        StageMaker.Instance.GridSizeUP(level);
    }

    public void ApplyCard(CardData card)
    {
        switch (card.cardType)
        {
            case CardType.StatUp:
                StatType statType = card.statCardData.statType;
                InvestStatPoint(statType);
                break;
            case CardType.Item:
                break;
            case CardType.Debuff:
                DebuffType debuffType = card.debuffCardData.debuffType;
                float amount = Random.Range(card.debuffCardData.minAmount,
                card.debuffCardData.mixAmount);
                switch (debuffType)
                {
                    case DebuffType.ObjHp:
                    case DebuffType.ObjLivingTime:
                    case DebuffType.ObjReBuildTime:
                    case DebuffType.ObjSpawnTime:
                    case DebuffType.ObjWarningTime:
                        ObjectManager.Instance.InvsetObjStatPoint(debuffType, amount);
                        break;
                    case DebuffType.WallHp:
                    case DebuffType.WallBuildTime:
                        WallManager.Instance.InvsetWallStatPoint(debuffType, amount);
                        break;
                }
                break;
        }
    }
    
    public void InvestStatPoint(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHP:
                hpCount++;
                if(hpCount%2==0)    runBonus.maxHP+=1;
                break;
            case StatType.Luck:
                runBonus.luck +=0.05f;
                break;
            case StatType.MoveSpeed:
                runBonus.moveSpeed += 2f;
                break;
            case StatType.ObjectAttack:
                runBonus.objectAttack += 2f;
                break;
            case StatType.WallAttack:
                runBonus.wallAttack += 2f;
                break;
            case StatType.AttackSpeed:
                runBonus.attackSpeed += 0.1f;
                break;
        }
    }

    
    public void OnPlayerDead()
    {
        runBonus.Reset();
    }
}