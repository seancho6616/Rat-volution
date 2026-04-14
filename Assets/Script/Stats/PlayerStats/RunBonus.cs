using UnityEngine;

public class RunBonus
{
    public int maxHP =0;
    public int wallCount = 0;
    public float moveSpeed = 0f;
    public float attackSpeed =0f;
    public float objectAttack =0f;
    public float wallAttack =0f;
    public float luck =0f;
    public float insight =0f;

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
