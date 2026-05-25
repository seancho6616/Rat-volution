using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    private PlayerControl playerControl;
    private Transform meshTransform;
    [SerializeField] private GameObject sword1;
    [SerializeField] private GameObject sword2;
    [SerializeField] private float swordShowDuration = 0.2f;  // 검 표시 시간

    [Header("Attack Settings")]
    public Vector3 attackBoxSize = new Vector3(15f, 10f, 15f);
    public float attackOffset = 7f;

    private float lastAttackTime;
    private Coroutine swordCoroutine;

    private void Start()
    {
        playerControl = GetComponent<PlayerControl>();
        if (playerControl != null)
        {
            meshTransform = playerControl.meshTransform;
        }

        // 시작 시 검 비활성화
        if (sword1 != null) sword1.SetActive(false);
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;
        if (playerControl == null || !playerControl.enabled) return;

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

        Vector3 attackCenter = transform.position + (meshTransform.forward * attackOffset);

        // 검 이펙트 표시 (코루틴으로)
        // if (sword1 != null)
        // {
        //     if (swordCoroutine != null)
        //         StopCoroutine(swordCoroutine);

        //     swordCoroutine = StartCoroutine(ShowSwordRoutine(transform.position));
        // }

        // 공격 범위 내의 레이어 탐색
        Collider[] hitObjects = Physics.OverlapBox(attackCenter, attackBoxSize / 2f, meshTransform.rotation, playerControl.objectLayer);

        if (hitObjects.Length == 0)
        {
            Debug.Log("공격 범위 내에 대상이 없습니다.");
        }

        PlayerSkill skill = GetComponent<PlayerSkill>();

        foreach (var col in hitObjects)
        {
            FallingObject target = col.GetComponent<FallingObject>();
            if (target != null)
            {
                float finalDamage = PlayerStats.Instance.FinalObjectAttack;
                target.TakeDamage(finalDamage);

                if (skill != null)
                {
                    skill.TryApplyDoT(target);
                    if (skill.CheckRapidStrike())
                    {
                        float rapidDamagePercent = Random.Range(0.2f, 0.5f);
                        float rapidDamage = finalDamage * rapidDamagePercent;

                        target.TakeDamage(rapidDamage);
                        Debug.Log($"연속 공격! 추가 데미지: {rapidDamage:F1}");
                    }
                }
            }
        }
    }

    private IEnumerator ShowSwordRoutine(Vector3 position)
    {
        sword1.transform.position = position;
        sword1.transform.rotation = meshTransform.rotation;  // 방향도 맞춰주면 자연스러움
        sword1.SetActive(true);

        yield return new WaitForSeconds(swordShowDuration);

        sword1.SetActive(false);
        swordCoroutine = null;
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