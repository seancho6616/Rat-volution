using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControl : PlayerStats
{
    [Header("Stats")]
    public int maxHeart => FinalMaxHP;
    public int currentHeart;
    [Header("UI Settings")]
    public Image[] heartImages;
    [Header("Movement Settings")]
    public float gridSize = 10f;       // 한 칸의 길이 10
    public float moveTime = 1.25f;    // 이동 속도 0.8칸/초 기준 (1 / 0.8 = 1.25초 소요)
    private bool isMoving = false;    // 이동 중 중복 입력 방지
    // 이동 제한 범위 설정 (중앙 기준으로 ±20 범위)
    [Header("Boundary Settings")]
    public float moveLimit = 32f;
    public float centerX = 8.70429f;
    public float centerZ = 0.70631f;

    [Header("Attack Settings")]
    public float attackDamage = 1f;
    public float autoAttackRange = 15f; // 3x3 범위 (그리드가 10이므로 반경 15 정도)
    public LayerMask wallLayer;
    public LayerMask objectLayer;

    private Vector2 moveInput;

    private void Start()
    {
        currentHeart = maxHeart;
        UpdateHeartUI();
        centerX = transform.position.x;
        centerZ = transform.position.z;
    }
    // Input System에서 Move 액션이 시작될 때(Started) 호출

    public void TakeDamage(int damage)
    {
        currentHeart -= damage;
        Debug.Log($"[Player] 목숨 -1, 남은 목숨: {currentHeart}");

        UpdateHeartUI();
        if (currentHeart <= 0)
        {
            RestartGame();
        }
    }

    // 체력 UI 업데이트 로직
    private void UpdateHeartUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            // 현재 체력보다 인덱스가 작으면 하트를 켜고, 크거나 같으면 끕니다.
            if (i < currentHeart)
            {
                heartImages[i].enabled = true;
            }
            else
            {
                heartImages[i].enabled = false;
            }
        }
    }

    private void RestartGame()
    {
        Debug.Log("[Player] 게임 재시작");
        // 현재 활성화된 씬을 다시 로드함
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnMove(InputValue value)
    {
        // 이미 이동 중이면 새로운 입력을 무시함 (Key Down 시 1회 이동 보장)
        if (isMoving) return;

        moveInput = value.Get<Vector2>();

        if (moveInput != Vector2.zero)
        {
            // 대각선 방지 및 방향 확정
            Vector3 direction = GetDirection(moveInput);
            StartCoroutine(TryMove(direction));
        }
    }

    private Vector3 GetDirection(Vector2 input)
    {
        // 절대값 비교 로직: 더 강한 입력 쪽으로 방향 확정
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            return new Vector3(input.x > 0 ? 1 : -1, 0, 0);
        }
        else
        {
            return new Vector3(0, 0, input.y > 0 ? 1 : -1);
        }
    }

    private IEnumerator TryMove(Vector3 direction)
{
    isMoving = true;
    Vector3 startPosition = transform.position;
    Vector3 targetPosition = startPosition + (direction * gridSize);

    // 바닥의 중심으로부터 거리 계산
    float distanceFromCenterX = Mathf.Abs(targetPosition.x - centerX);
    float distanceFromCenterZ = Mathf.Abs(targetPosition.z - centerZ);

    if (distanceFromCenterX > moveLimit || distanceFromCenterZ > moveLimit)
    {
        Debug.Log("[Player] 판 밖으로 이동 시도 - 이동 불가");
        yield return StartCoroutine(BumpAndReturn(startPosition, direction));
        isMoving = false;
        yield break;
    }

    if (Physics.Raycast(startPosition, direction, out RaycastHit hit, gridSize * 1.1f, wallLayer))
{
    Wall wall = hit.collider.GetComponent<Wall>();
    if (wall != null)
    {
        wall.TakeDamage(1); // 1 데미지
    }

    yield return StartCoroutine(BumpAndReturn(startPosition, direction));
    isMoving = false;
    yield break;
}

    // 정상 이동
    yield return StartCoroutine(SmoothMove(startPosition, targetPosition));
    isMoving = false;
}

private IEnumerator BumpAndReturn(Vector3 startPosition, Vector3 direction)
{
    // 앞으로 절반 이동
    Vector3 bumpTarget = startPosition + direction * (gridSize * 0.4f);

    float elapsed = 0f;
    float bumpTime = moveTime * 0.25f; // 빠르게 치고 나가기

    while (elapsed < bumpTime)
    {
        transform.position = Vector3.Lerp(startPosition, bumpTarget, elapsed / bumpTime);
        elapsed += Time.deltaTime;
        yield return null;
    }

    // 원래 위치로 복귀
    elapsed = 0f;
    float returnTime = moveTime * 0.35f;

    while (elapsed < returnTime)
    {
        transform.position = Vector3.Lerp(bumpTarget, startPosition, elapsed / returnTime);
        elapsed += Time.deltaTime;
        yield return null;
    }

    transform.position = startPosition; // 정확히 원위치 스냅
}

private IEnumerator SmoothMove(Vector3 from, Vector3 to)
{
    float elapsed = 0f;
    float duration = moveTime * Vector3.Distance(from, to) / gridSize; // 거리 비례 시간

    while (elapsed < duration)
    {
        transform.position = Vector3.Lerp(from, to, elapsed / duration);
        elapsed += Time.deltaTime;
        yield return null;
    }

    transform.position = to;
}

    // private void PerformAutoAttack()
    // {
    //     // 3x3 범위 탐색 (그리드가 10이므로 주변 10씩 총 30x30 범위를 체크)
    //     Collider[] hitObjects = Physics.OverlapBox(transform.position, new Vector3(15f, 1f, 15f), Quaternion.identity, objectLayer);

    //     foreach (var col in hitObjects)
    //     {
    //         IDamageable target = col.GetComponent<IDamageable>();
    //         if (target != null)
    //         {
    //             target.TakeDamage(attackDamage);
    //         }
    //     }
    // }

    private void OnDrawGizmos()
    {
        // 자동 공격 범위 시각화 (3x3 칸)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(30f, 1f, 30f));
    }
}
