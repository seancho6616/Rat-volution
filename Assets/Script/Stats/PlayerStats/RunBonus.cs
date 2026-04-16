using UnityEngine;

public class RunBonus
{
    public int   maxHP;
    public int   wallCount;
    public float moveSpeed;
    public float attackSpeed;
    public float objectAttack;
    public float wallAttack;
    public float luck;
    public float insight;

    public void Reset()
    {
        maxHP       = 0;
        wallCount   = 0;
        moveSpeed   = 0f;
        attackSpeed = 0f;
        objectAttack= 0f;
        wallAttack  = 0f;
        luck        = 0f;
        insight     = 0f;
    }
}
