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

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void GainCheese(float amount)
    {
        currentCheese += amount;
        if (currentCheese >= maxCheese)
            LevelUP();
    }

    private void LevelUP()
    {
        level++;
        currentCheese = 0;
        maxCheese += 5;
    }
}