using UnityEngine;
using System.Collections;
using Mono.Cecil.Cil;

public class PlayerSkill : MonoBehaviour
{

    [Header("Skills States")]
    public float lastUsedTime = -999f; // 마지막 점프 사용 시간
    private Coroutine adrenalineCoroutine;
    private Coroutine slowMotionCoroutine;


    public void UseMagnet(float range = 10f)
    {
        // 1칸 범위 내 치즈 탐색 후 플레이어 위치로 이동
        if (!PlayerStats.Instance.item.magnet) return;
        Debug.Log("자석 스킬 사용");
        Collider[] cheeses = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Cheese"));
        foreach (var col in cheeses)
        {
            if (col != null)
            {
                StartCoroutine(PullCheese(col.transform));
            }
        }
    }

    private IEnumerator PullCheese(Transform cheese)
    {
        while (cheese != null && Vector3.Distance(transform.position, cheese.position) > 0.5f)
        {
            cheese.position = Vector3.MoveTowards(cheese.position, transform.position, 25f * Time.deltaTime);
            yield return null;
        }
    } 
    // 벽 넘기 스킬 사용 여부 - 특수 이동
    public bool TryUseJump()
    {
        float coolTime = PlayerStats.Instance.item.specialMove; // 특수 이동 스킬 사용 시 활성화
        if (coolTime == 0) return false; // 스킬이 없는 경우

        if (Time.time < lastUsedTime + coolTime)
        {
            float remain = (lastUsedTime + coolTime) - Time.time;
            Debug.Log($"쿨타임 중: {remain:F1}초");
            return false; // 아직 쿨타임이 끝나지 않음
        }
        lastUsedTime = Time.time; // 사용 시간 기록
        Debug.Log("벽 넘기 스킬 사용");
        return true; // 스킬 사용 가능
    }

    // [날카로운 앞니] 확률 계산
    public bool CheckSharpFangs()
    {
        float chance = PlayerStats.Instance.item.sharpFangs;

        if (chance <= 0f) return false; // 스킬이 없는 경우

        // 인벤토리에 저장된 수치에 따라 확률 계산
        if (Random.value < chance)
        {
            Debug.Log("날카로운 앞니 효과 발동! 추가 치즈 획득!");
            return true; // 효과 발동
        }
        return false;
    }

    // 슬로우 모션 활성화
    public void ActivateSlowMotion(float amount)
    {
        if (!PlayerStats.Instance.item.slowMotion) return;
        if (slowMotionCoroutine != null)
            StopCoroutine(slowMotionCoroutine);
        slowMotionCoroutine = StartCoroutine(SlowMotionRoutine(4f, amount)); // 4초 동안 슬로우 모션 효과 적용
    }
    private IEnumerator SlowMotionRoutine(float duration, float amount)
    {
        Debug.Log("슬로우 모션 활성화");

        Time.timeScale = Mathf.Clamp(1f - amount, 0.1f, 1f);
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // 물리 업데이트도 조정
        Debug.Log($"슬로우 모션 적용: {amount * 100}% 느려짐");

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f; // 원래 속도로 복구
        Time.fixedDeltaTime = 0.02f; // 물리 업데이트 원래대로
        Debug.Log("슬로우 모션 비활성화");
    }

    public void TriggerAdrenalineRush()
    {
        if (!PlayerStats.Instance.item.adrenaline) return;
        
        if (adrenalineCoroutine != null)
            StopCoroutine(adrenalineCoroutine);
        adrenalineCoroutine = StartCoroutine(AdrenalineRushRoutine()); // 아드레날린 러시 지속 시간 동안 효과 적용
    }

    IEnumerator AdrenalineRushRoutine()
    {
        Debug.Log("아드레날린 러시 활성화");
        // 임시 이동속도 보너스 적용
        float boostAmount = PlayerStats.Instance.baseData.moveSpeed * 0.5f; // 기본 이동속도의 50% 추가

        PlayerStats.Instance.runBonus.moveSpeed += boostAmount;

        yield return new WaitForSeconds(2f); // 2초 동안 지속
        PlayerStats.Instance.runBonus.moveSpeed -= boostAmount; // 보너스 제거
        Debug.Log("아드레날린 러시 비활성화");
    }

    public bool CheckLuckySeven()
    {
        if (PlayerStats.Instance.item.luckySeven <= 0f) return false;

        float luckBonus = PlayerStats.Instance.item.luckySeven + (0.1f * PlayerStats.Instance.FinalLuck);
        if (Random.value < luckBonus)
        {
            Debug.Log("777 효과 발동! 치즈 3개 추가 획득!");
            return true;
        }
        return false;
    }

    // 독 데미지 적용
    public void TryApplyDoT(FallingObject target)
    {
        // DoT 효과가 활성화된 경우, 대상에게 독 데미지 적용
        float dotPercent = PlayerStats.Instance.item.dot;

        if (dotPercent <= 0f || target == null) return; // 스킬이 없거나 대상이 없는 경우
        
        float damage = PlayerStats.Instance.FinalObjectAttack * dotPercent; // DoT 데미지 계산
        StartCoroutine(DoTRoutine(target, damage, 3f)); // 3초 동안 DoT 효과 적용
    }

    private IEnumerator DoTRoutine(FallingObject target, float damage, float duration)
    {
        float elapsed = 0f;
        float tickInterval = 1f; // 1초마다 데미지 적용

        while (elapsed < duration && target != null)
        {
            yield return new WaitForSeconds(tickInterval);
            elapsed += tickInterval;

            if (target != null)
            {
                target.TakeDamage(damage);
                Debug.Log($"DoT 효과: {damage} 데미지 적용 (남은 시간: {duration - elapsed:F1}초)");
            }
        }
    }

    // 연속 공격 Rapid Strike
    public bool CheckRapidStrike()
    {
        float rapidChance = PlayerStats.Instance.item.rapidStrike;
        if (rapidChance <= 0f) return false;

        // 연속 공격 확률 계산
        if (Random.value < rapidChance)
        {
            Debug.Log("연속 공격 효과 발동! 추가 공격 기회!");
            return true; // 연속 공격 발동
        }
        return false;
    }

    // 악마와의 계약 DealwithDevil
    public bool TryResurrect()
    {
        if (!PlayerStats.Instance.item.dealWithDevil) return false;

        Debug.Log("악마와의 계약 발동 - 즉시 부활");
        
        // 부활 시 체력 회복 등 추가 효과 적용 가능
        float currentTotalCheese = PlayerStats.Instance.totalCheese + PlayerStats.Instance.currentCheese;
        int cheeseCost = Mathf.RoundToInt(currentTotalCheese * 0.15f);

        PlayerStats.Instance.currentCheese -= cheeseCost;

        if (PlayerStats.Instance.currentCheese < 0)
        {
            PlayerStats.Instance.totalCheese += PlayerStats.Instance.currentCheese; // 부족한 치즈만큼 총 치즈에서 차감
            PlayerStats.Instance.currentCheese = 0;
        }
        PlayerStats.Instance.item.dealWithDevil = false; // 계약은 한 번만 발동

        return true; // 부활 성공
    }
}
