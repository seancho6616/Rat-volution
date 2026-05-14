using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerControl playerControl;
    private Transform meshTransform;

    [Header("Attack Settings")]
    public Vector3 attackBoxSize = new Vector3(10f, 5f, 10f);
    public float attackOffset = 7f;

    private float lastAttackTime;

    private void Start()
    {
        playerControl = GetComponent<PlayerControl>();
        if (playerControl != null)
        {
            meshTransform = playerControl.meshTransform;
        }
    }

    private void Update()
    {
        // 1. 마우스 왼쪽 버튼 클릭 시 공격 시도
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        float attackInterval = 1f / PlayerStats.Instance.FinalAttackSpeed;

        if (Time.time >= lastAttackTime + attackInterval)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
        else
        {
            // 쿨타임 로그
            Debug.Log($"공격 쿨타임: {attackInterval - (Time.time - lastAttackTime):F2}초 남음");
        }
    }

    private void PerformAttack()
    {
        if (meshTransform == null)
        {
            Debug.LogWarning("Mesh Transform이 설정되지 않았습니다.");
            return;
        }
        // 바라보고 있는 방향으로 공격 중심점 계산
        Vector3 boxSize = new Vector3 (15f, 10f, 15f);
        Vector3 attackCenter = transform.position + (meshTransform.forward * attackOffset);

        // 공격 범위 내의 레이어 탐색
        Collider[] hitObjects = Physics.OverlapBox(attackCenter, boxSize / 2f, meshTransform.rotation, playerControl.objectLayer);

        if (hitObjects.Length == 0)
        {
            Debug.Log("공격 범위 내에 대상이 없습니다.");
        }

        foreach (var col in hitObjects)
        {
            FallingObject target = col.GetComponent<FallingObject>();
            if (target != null)
            {
                target.TakeDamage(PlayerStats.Instance.FinalObjectAttack);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerControl == null || playerControl.meshTransform == null) return;

        Gizmos.color = Color.red;
        Vector3 attackCenter = transform.position + (playerControl.meshTransform.forward * attackOffset);
        Gizmos.matrix = Matrix4x4.TRS(attackCenter, playerControl.meshTransform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, attackBoxSize);
    }
}
