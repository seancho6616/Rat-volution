using UnityEngine;
using System.Collections;

public class PlayerSkill : MonoBehaviour
{

    [Header("Skills States")]
    public bool canJumpOverWall = false; // 특수 이동
    public float lastUsedTime = -999f; // 마지막 점프 사용 시간
    public bool isSharpFangsActive = false; // 날카로운 앞니 활성화 여부
    public bool isSlowMotionActive = false; // 슬로우 모션 활성화 여부
    public bool hasAdrenalineRush = false; // 아드레날린 러시 활성화 여부
    public bool hasDoT = false; // 독 데미지 활성화 여부
    public float dotDamage = 0f; // DoT 데미지 양
    public bool hasRapidStrike = false; // 연속 공격 활성화 여부
    public bool hasLuckySeven = false; // 777 활성화 여부
    public bool hasDealWithDevil = false; // 악마와 거래 활성화 여부
    private Coroutine adrenalineCoroutine;
    private Coroutine slowMotionCoroutine;


    public void UseMagnet(float range = 10f)
    {
        // 1칸 범위 내 치즈 탐색 후 플레이어 위치로 이동
        Debug.Log("자석 스킬 사용");
        Collider[] cheeses = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Cheese"));
        foreach (var col in cheeses)
        {
            StartCoroutine(PullCheese(col.transform));
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
    public void ActivateSharpFangs()
    {
        isSharpFangsActive = true; // 일단 활성화 상태로 설정 (실제 확률 계산은 공격 시점에 적용)
        Debug.Log("날카로운 앞니 활성화");
        // float chance = 0.1f; // 10% 확률
        // if (Random.value < chance)
        // {
        //     isSharpFangsActive = true;
        //     Debug.Log("날카로운 앞니가 활성화되었습니다!");
        // }
        // else
        // {
        //     isSharpFangsActive = false;
        //     Debug.Log("날카로운 앞니가 활성화되지 않았습니다.");
        // }
    }

    // 슬로우 모션 활성화
    public void ActivateSlowMotion(float amount)
    {
        if (slowMotionCoroutine != null)
            StopCoroutine(slowMotionCoroutine);
        slowMotionCoroutine = StartCoroutine(SlowMotionRoutine(4f, amount)); // 4초 동안 슬로우 모션 효과 적용}
    }
    private IEnumerator SlowMotionRoutine(float duration, float amount)
    {
        isSlowMotionActive = true;
        Debug.Log("슬로우 모션 활성화");
        float originalTimeScale = Time.timeScale;
        // 게임 속도 느리게
        Time.timeScale = 1f - amount;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // 물리 업데이트도 조정
        Debug.Log($"슬로우 모션 적용: {amount * 100}% 느려짐");

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f; // 원래 속도로 복구
        Time.fixedDeltaTime = 0.02f; // 물리 업데이트 원래대로
        isSlowMotionActive = false;
        Debug.Log("슬로우 모션 비활성화");
    }

    // public void TriggerAdrenalineRush()
    // {
    //     if (!hasAdrenalineRush) return;

    //     if (adrenalineCoroutine != null)
    //         StopCoroutine(adrenalineCoroutine);
    //     adrenalineCoroutine = StartCoroutine(AdrenalineRushRoutine()); // 아드레날린 러시 지속 시간 동안 효과 적용
    // }

    // IEnumerator AdrenalineRushRoutine()
    // {
    //     Debug.Log("아드레날린 러시 활성화");
    //     // 임시 이동속도 보너스 적용
    //     float originalBonus = playerStats.runBonus.moveSpeed;
    //     float boostAmount = playerStats.baseData.moveSpeed * 0.5f; // 기본 이동속도의 50% 추가

    //     playerStats.runBonus.moveSpeed += boostAmount;

    //     yield return new WaitForSeconds(2f); // 2초 동안 지속
    //     playerStats.runBonus.moveSpeed -= boostAmount; // 보너스 제거
    //     Debug.Log("아드레날린 러시 비활성화");
    // }

    // public bool CheckLuckySeven()
    // {
    //     if (!hasLuckySeven) return false;

    //     float luckBonus =0.07f + (0.1f * playerStats.FinalLuck);
    //     if (Random.value < luckBonus)
    //     {
    //         Debug.Log("777 효과 발동! 치즈 3개 추가 획득!");
    //         return true;
    //     }
    //     return false;
    // }
}
