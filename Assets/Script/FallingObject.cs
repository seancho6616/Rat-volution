using UnityEngine;
using System.Collections;
using System.Linq.Expressions;

public class FallingObject : MonoBehaviour, IDamageable
{
    private Renderer objectColor;
    public enum ObjectState { Warning, Falling, Grounded }
    public ObjectState CurrentState { get; private set; }

    [Header("Stats")]
    public float maxHealth = 10f;
    private float currentHealth;
    // private bool isDestroyedByPlayer = false;

    [Header("Grid Settings")]
    private float gridSize = 10f; // 한 칸의 길이

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    private void Awake()
    {
        objectColor = gameObject.GetComponent<Renderer>();
    }

    public void Init(float hp, float gSize)
    {
        this.maxHealth = hp;
        this.currentHealth = hp;
        this.gridSize = gSize;
         if (objectColor == null)
            objectColor = GetComponent<Renderer>();
        StartCoroutine(LifecycleRoutine());
    }

    private IEnumerator LifecycleRoutine()
    {
        // 1. 생성 단계
        CurrentState = ObjectState.Warning;
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled =false;
        SetVisualAlpha(0f); 
        yield return new WaitForSeconds(1f);
        meshRenderer.enabled = true;
        // 2. 예고 단계
        SetVisualAlpha(0.3f); // 반투명한 그림자 상태
        yield return new WaitForSeconds(3f);

        // 2. 낙하 단계 (1초)
        CurrentState = ObjectState.Falling;
        SetVisualAlpha(1.0f);
        Vector3 startPos = transform.position + Vector3.up * 35f;
        Vector3 endPos = transform.position;
        
        float elapsed = 0;
        while (elapsed < 1f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / 1f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;

        // 3. 착지 시 벽 충돌 체크 (2x2 범위 = 20x20 유닛)
        CheckWallCollision();

        // 4. 유지 단계 (3초)
        CurrentState = ObjectState.Grounded;
        yield return new WaitForSeconds(3f);

        if (CurrentState == ObjectState.Grounded) 
            DestroyObject(false);
    }

    // 플레이어와 부딪혔을 때 처리
    private void OnTriggerEnter(Collider other)
    {
        // 낙하 중이 아닐 때는 충돌 무시
        if (CurrentState != ObjectState.Falling) return;
        // 우선순위 벽으로 설정
        if (other.CompareTag("Wall"))
        {
            Wall wall = other.GetComponent<Wall>();
            if (wall != null)
            {
                // wall.TakeDamage(999); // 벽에 데미지를 주고 즉시 파괴
                wall.InstantDestroy();
                CurrentState = ObjectState.Grounded; // 상태가 변경되어 더이상 공격하지 않음
                return;
            }
        }

        if (other.CompareTag("Player"))
        {
            PlayerHitEffect hitEffect = other.GetComponent<PlayerHitEffect>();
            if (hitEffect != null)
            {
                hitEffect.PlayHitEffect();
            }
            PlayerControl player = other.GetComponent<PlayerControl>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
            Debug.Log("플레이어 공격, 데미지 -1");
        }
    }

    private void CheckWallCollision()
    {
        // 2x2 범위를 체크 (중심점에서 각 방향으로 10유닛씩)
        Collider[] hitWalls = Physics.OverlapBox(transform.position, new Vector3(9f, 5f, 9f), Quaternion.identity, LayerMask.GetMask("Wall"));
        
        if (hitWalls.Length > 0)
        {
            foreach (var wall in hitWalls)
            {
                wall.GetComponent<IDamageable>()?.DestroyObject();
            }
            DestroyObject(false); // 벽과 충돌 시 자신도 파괴
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) DestroyObject(true);
    }

    public void DestroyObject() => DestroyObject(true);

    private void DestroyObject(bool byPlayer)
    {
        StopAllCoroutines();
        ObjectManager.Instance.OnObjectRemoved(gameObject, byPlayer);
        Destroy(gameObject);
    }

    public void SetVisualAlpha(float alpha)
    {
        if (objectColor != null && objectColor.material != null)
        {
            Color color = objectColor.material.color;
            color.a = alpha;
            objectColor.material.color = color;
        }
    }
}