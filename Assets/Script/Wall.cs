// Wall.cs
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private int maxHp = 3;
    private int currentHp;
    private Vector3 spawnPos;

    private bool isInvincible = false;
    private float invincivilityDuration = 0.5f; // 무적 시간

    public void Init(Vector3 pos)
    {
        spawnPos = pos;
        // 벽이 생성될 때 HP 초기화
        currentHp = maxHp;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 플레이어와 충돌 시 데미지 1
        if (collision.gameObject.CompareTag("Player"))
        {
            TakeDamage(1);
        }
    }

    public void InstantDestroy()
    {
        if (WallManager.Instance != null)
        {
            // WallManager의 디렉토리에서 해당 위치를 다시 스폰 가능하게 함
            WallManager.Instance.ReleaseWall(gameObject);
        }
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // 무적 상태면 데미지 무시

        currentHp -= damage;
        Debug.Log($"[Wall] HP: {currentHp}/{maxHp}");

        if (currentHp <= 0)
        {
            InstantDestroy();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincivilityDuration);
        isInvincible = false;
    }
}