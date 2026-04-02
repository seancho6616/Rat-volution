// Wall.cs
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private int maxHp = 3;
    private int currentHp;
    private Vector3 spawnPos;

    public void Init(Vector3 pos)
    {
        spawnPos = pos;
    }

    public void TakeDamage(int damage = 1)
    {
        currentHp -= damage;
        Debug.Log($"[Wall] HP: {currentHp}/{maxHp}");

        if (currentHp <= 0)
        {
            WallManager.Instance.ReleaseWall(spawnPos);
            Destroy(gameObject);
        }
    }
}