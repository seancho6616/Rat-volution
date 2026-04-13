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

    public int finalMaxHP =>baseData.maxHP + runBonus.maxHP;
    public int finalWallCount => baseData.wallCount + runBonus.wallCount;
    public float finalMoveSpeed => baseData.moveSpeed + runBonus.moveSpeed;
    public float finalAttackSpeed => baseData.attackSpeed + runBonus.attackSpeed;
    public float finalObjectAttack => baseData.objectAttack + runBonus.objectAttack;
    public float finalWallAttack => baseData.wallAttack + runBonus.wallAttack;
    public float finalLuck => baseData.luck + runBonus.luck;
    public float finalInsight => baseData.insight + runBonus.insight;

    [Header("치즈 개수")]
    public float maxCheese = 49;
    public float currentCheese =0;
    public int level = 1;

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