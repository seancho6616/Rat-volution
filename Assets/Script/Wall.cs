// Wall.cs
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] public int maxHp;
    private int currentHp;
    private Vector3 spawnPos;

    private bool isInvincible = false;
    private float invincivilityDuration = 0.5f; // 무적 시간
    [SerializeField] private Wall_HitFlashEffect wall_HitFlashEffect;

    [Header("Sound Settings")]
    public AudioClip destroySound; // 벽이 부서질 때 날 소리
    [Range(0f, 1f)] public float soundVolume = 0.5f; // 볼륨 조절 슬라이더

    public void Init(Vector3 pos)
    {
        spawnPos = pos;

        if (WallManager.Instance != null)
        {
            this.maxHp = WallManager.Instance.Finalhp;
        }
        // 벽이 생성될 때 HP 초기화
        currentHp = maxHp;
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     // 플레이어와 충돌 시 데미지 1
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         TakeDamage(1);
    //     }
    // }

    public void InstantDestroy()
    {
        // --- 파괴 사운드 재생 (오브젝트가 사라져도 소리가 끝까지 재생됨) ---
        if (destroySound != null)
        {
            // 카메라 위치에서 재생시켜 거리에 상관없이 선명하게 들리도록 처리
            if (Camera.main != null)
            {
                AudioSource.PlayClipAtPoint(destroySound, Camera.main.transform.position, soundVolume);
            }
            else
            {
                AudioSource.PlayClipAtPoint(destroySound, transform.position, soundVolume);
            }
        }
        
        Collider WallCollider = GetComponent<Collider>();
        if (WallCollider != null)
        {
            WallCollider.enabled = false; // 충돌 비활성화
        }
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

        wall_HitFlashEffect.Flash();
        currentHp -= damage;
        // Debug.Log($"[Wall] HP: {currentHp}/{maxHp}");

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