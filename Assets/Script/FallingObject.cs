using UnityEngine;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine.Rendering;

public class FallingObject : MonoBehaviour, IDamageable
{
    private Renderer objectColor;
    public ParticleSystem groundParticle;
    public enum ObjectState { Warning, Falling, Grounded }
    public ObjectState CurrentState { get; private set; }

    [Header("Sound Settings")]
    public AudioClip dropSound; // 바닥에 닿을 때 나는 소리
    [Range(0f, 1f)] public float dropVolume = 0.5f;

    [Header("Stats")]
    public float maxHealth;
    private float currentHealth;
    // private bool isDestroyedByPlayer = false;

    // private ObjectData objectData;

    private Color originalColor;

    [Header("Grid Settings")]
    private float gridSize = 10f; // 한 칸의 길이

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    private float fallingSpeedMultiplier = 1f;

    
    private void Awake()
    {
        objectColor = gameObject.GetComponent<Renderer>();

        // 게임 시작 시 초기 색 백업
        if (objectColor != null && objectColor.material != null)
        {
            originalColor = objectColor.material.GetColor("_BaseColor");
        }
    }

    public void Init(float hp, float gSize)
    {
        fallingSpeedMultiplier = 1f;

        if (ObjectManager.Instance != null)
        {
            this.maxHealth = ObjectManager.Instance.Finalhp;
            this.currentHealth = ObjectManager.Instance.Finalhp;
        }
        else
        {
            this.maxHealth = hp;
            this.currentHealth = hp;
        }

        this.gridSize = gSize;

        if (objectColor == null) objectColor = GetComponent<Renderer>();
        // 오브젝트 초기화 시 색상과 투명도 설정
        if (objectColor != null && objectColor.material != null)
        {
            objectColor.material.SetColor("_BaseColor", originalColor);
        }
        StartCoroutine(LifecycleRoutine());
    }
    public void SetFallSpeed(float multiplier)
    {
        fallingSpeedMultiplier = multiplier;
    }

    // --- 파티클과 소리를 한 번에 재생하는 함수 ---
    private void PlayGroundEffect()
    {
        if (groundParticle != null)
        {
            groundParticle.Clear();
            groundParticle.Play();
        }

        if (dropSound != null)
        {
            if (Camera.main != null)
                AudioSource.PlayClipAtPoint(dropSound, Camera.main.transform.position, dropVolume);
            else
                AudioSource.PlayClipAtPoint(dropSound, transform.position, dropVolume);
        }
    }

    private IEnumerator LifecycleRoutine()
    {
        while (true)
        {
            // 1. 생성 단계
            CurrentState = ObjectState.Warning;
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled =false;
            SetVisualAlpha(0f);

            // objectData랑 연결된 objectManager로 반영
            float randomDelay = 1f;
            if (ObjectManager.Instance != null)
            {
                randomDelay = Random.Range(ObjectManager.Instance.FinalMinSpawnTime, ObjectManager.Instance.FinalMaxSpawnTime);
            }
            yield return new WaitForSeconds(randomDelay);
            meshRenderer.enabled = true;

            // 2. 예고 단계
            SetVisualAlpha(0.3f); // 반투명한 그림자 상태
            float randomDuration = 3f;
            if (ObjectManager.Instance != null)
            {
                randomDuration = Random.Range(ObjectManager.Instance.FinalMinWarningTime, ObjectManager.Instance.FinalMaxWarningTime);
            }
            yield return new WaitForSeconds(randomDuration);

            // 3. 낙하 단계 (1초)
            CurrentState = ObjectState.Falling;
            SetVisualAlpha(1.0f);

            // 슬로우모션 코드 발동
            float slowAmount = 0f;
            foreach (var pair in Inventory.Instance.itemCheck)
            {
                if (pair.Key.cardName == "SlowMotion")
                {
                    slowAmount = pair.Key.amount;
                    break;
                }
            }
            PlayerSkill skill = FindAnyObjectByType<PlayerSkill>();
            skill?.ActivateSlowMotion(slowAmount);

            Vector3 startPos = transform.position + Vector3.up * 35f;
            Vector3 endPos = transform.position;
            
            float elapsed = 0;
            float fallDuration = Mathf.Max(ObjectManager.Instance.baseData.fallingTime, 0.3f) / fallingSpeedMultiplier;
            while (elapsed < fallDuration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, elapsed / fallDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = endPos;

            //이펙트 + 소리 재생
            if (CurrentState == ObjectState.Falling) 
            {
                PlayGroundEffect();
            }

            // 3. 착지 시 벽 충돌 체크 (2x2 범위 = 20x20 유닛)
            CheckWallCollision();

            // 5. 유지 단계 (3초)
            CurrentState = ObjectState.Grounded;
            yield return new WaitForSeconds(3f);
            // 6. 상승 단계
            elapsed = 0;
            while (elapsed < 1f)
            {
                transform.position = Vector3.Lerp(endPos, startPos, elapsed / 1f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = startPos;

            // 위치 이동
            ObjectManager.Instance.ReleasePosition(endPos);

            Vector3 nextPos = ObjectManager.Instance.GetNextSpawnPosition();
            if (nextPos != Vector3.zero)
            {
                transform.position = nextPos;
            }
            // if (CurrentState == ObjectState.Grounded) 
            //     DestroyObject(false);
        }
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
                Bomb.Instance.ParticlePlay(transform.position);
                // Debug.Log("벽과 직접 충돌 - 둘 다 파괴");
                wall.InstantDestroy(); // 벽 즉시 파괴
                DestroyObject(false); // 자신도 파괴
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
            CurrentState = ObjectState.Grounded;
            // Debug.Log("플레이어 공격, 데미지 -1");
            
            PlayGroundEffect();
        }

        if (other.CompareTag("Ground"))
        {
            // 기존의 긴 코드 수정
            PlayGroundEffect(); 
        }
        
    }

    private void CheckWallCollision()
    {
        // 2x2 범위를 체크 (중심점에서 각 방향으로 10유닛씩)
        Collider[] hitWalls = Physics.OverlapBox(transform.position, new Vector3(4.5f, 5f, 4.5f), Quaternion.identity, LayerMask.GetMask("Wall"));
        
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
        // Debug.Log($"<color=red>[오브젝트 피격]</color> {gameObject.name} 남은 체력: {currentHealth} / {maxHealth}");

        // 체력에 따라 색상 변화 (예시: 체력이 50% 이하일 때 붉은색으로)
        UpdateObjectColor();

        if (currentHealth <= 0)
        {
            // Debug.Log($"<color=green>[오브젝트 파괴]</color> {gameObject.name}이(가) 파괴되었습니다.");
            DestroyObject(true);
        }
    }

    private void UpdateObjectColor()
    {
        if (objectColor != null && objectColor.material != null)
        {
            float hpRatio = currentHealth / maxHealth;
            Color targetColor = Color.Lerp(Color.red, originalColor, hpRatio);
            objectColor.material.SetColor("_BaseColor", targetColor);
        }
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
            objectColor.material.SetFloat("_Alpha", alpha);
        }
    }

    
}