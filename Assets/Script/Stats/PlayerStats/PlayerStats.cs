using UnityEngine;

public enum StatType
{
    None = -1,
    MaxHP, WallCount, Luck, Insight,
    MoveSpeed, ObjectAttack, AttackSpeed, WallAttack,
}

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    public PlayerBaseStatsData baseData;
    public RunBonus runBonus = new RunBonus();

    public int FinalMaxHP =>baseData.maxHP + (int)runBonus.maxHP;
    public int FinalWallCount => baseData.wallCount + runBonus.wallCount;
    public float FinalMoveSpeed => baseData.moveSpeed + runBonus.moveSpeed;
    public float FinalAttackSpeed => baseData.attackSpeed + runBonus.attackSpeed;
    public float FinalObjectAttack => baseData.objectAttack + runBonus.objectAttack;
    public float FinalWallAttack => baseData.wallAttack + runBonus.wallAttack;
    public float FinalLuck => baseData.luck + runBonus.luck;
    public float FinalInsight => baseData.insight + runBonus.insight;

    [Header("치즈 개수")]
    public float totalCheese;
    public float maxCheese = 49;
    public float currentCheese =0;
    public int level = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log($"PlayerStats Instance 설정됨: {gameObject.name}"); // ✅ 어떤 오브젝트인지 확인
        }
        else
        {
            Debug.LogWarning($"중복 PlayerStats 발견: {gameObject.name}"); // ✅ 중복 여부 확인
            Destroy(gameObject);
        }
        // runBonus.Reset();
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
        totalCheese += currentCheese;
        currentCheese = 0;
        maxCheese += 5;
        StageMaker.Instance.GridSizeUP(level);
    }

    // public void ApplyCard(CardData card)
    // {
    //     switch (card.cardType)
    //     {
    //         case CardType.StatUp:
    //             StatType statType = card.statType;
    //             InvestStatPoint(statType);
    //             break;
    //         case CardType.Item:
    //             break;
    //         case CardType.Debuff:
    //             DebuffType debuffType = card.debuffCardData.debuffType;
    //             float amount = Random.Range(card.debuffCardData.minAmount,
    //             card.debuffCardData.mixAmount);
    //             switch (debuffType)
    //             {
    //                 case DebuffType.ObjHp:
    //                 case DebuffType.ObjLivingTime:
    //                 case DebuffType.ObjReBuildTime:
    //                 case DebuffType.ObjSpawnTime:
    //                 case DebuffType.ObjWarningTime:
    //                     ObjectManager.Instance.InvsetObjStatPoint(debuffType, amount);
    //                     break;
    //                 case DebuffType.WallHp:
    //                 case DebuffType.WallBuildTime:
    //                     WallManager.Instance.InvsetWallStatPoint(debuffType, amount);
    //                     break;
    //             }
    //             break;
    //     }
    // }
    
    public void InvestStatPoint(StatType type)
    {
        if (runBonus == null)
        {
            Debug.LogError("runBonus가 null입니다!");
            return;
        }
        switch (type)
        {
            case StatType.MaxHP:
                runBonus.maxHP += 0.5f;
                break;
            case StatType.Luck:
                runBonus.luck +=0.05f;
                break;
            case StatType.MoveSpeed:
                runBonus.moveSpeed += 0.1f;
                Debug.Log($"moveSpeed 적용 후: {runBonus.moveSpeed}"); // 값 확인
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