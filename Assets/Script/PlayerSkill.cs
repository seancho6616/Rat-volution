using UnityEngine;
using System.Collections;
using NUnit.Framework;

public class PlayerSkill : MonoBehaviour
{
    public PlayerControl playerControl;
    public PlayerStats playerStats;

    [Header("Skills States")]
    public bool canJumpOverWall = false; // 특수 이동
    public bool isLegendaryJump = false; // 레전드 등급 여부
    public int jumpCount = 0; // 점프 횟수 (레전드 점프용)
    public int shieldCount = 0; // 일회성 보호막
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

    void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
        playerStats = GetComponent<PlayerStats>();
    }

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
    public void SetSpecialMove(bool legendary, int count)
    {
        canJumpOverWall = true;
        isLegendaryJump = legendary;
        
        if (!isLegendaryJump)
        {
            jumpCount = count; // 일반 점프 횟수 설정
            Debug.Log("특수 이동이 활성화되었습니다. 점프 횟수: " + count);
        }
        else
        {
            Debug.Log("레전드 점프가 활성화되었습니다. 점프 횟수: 무제한");
        }
    }

    public bool TryUseJump()
    {
        if (!canJumpOverWall) return false; // 스킬이 활성화되지 않음
        if (isLegendaryJump) return true; // 레전드 점프는 무제한 사용 가능

        // 일반 점프는 횟수 제한
        if (jumpCount > 0)
        {
            jumpCount--;
            Debug.Log("벽 넘기 점프 사용! 남은 점프 횟수: " + jumpCount);
            if (jumpCount <= 0)
            {
                canJumpOverWall = false; // 점프 횟수 소진 시 스킬 비활성화
                Debug.Log("점프 횟수가 모두 소진되었습니다. 특수 이동이 비활성화됩니다.");
            }
            return true; // 점프 사용 성공
        }
        return false; // 점프 사용 실패
    }

    // 일회성 보호막 획득
    public void AddShield(int count)
    {
        shieldCount += count;
        Debug.Log("보호막이 " + count + "개 추가되었습니다. 현재 보호막 수: " + shieldCount);
    }

    public bool CheckShield()
    {
        if (shieldCount > 0)
        {
            shieldCount--;
            Debug.Log("보호막이 발동되었습니다. 남은 보호막 수: " + shieldCount);
            return true; // 보호막이 발동됨
        }
        return false; // 보호막이 없음
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

    public void TriggerAdrenalineRush()
    {
        if (!hasAdrenalineRush) return;

        if (adrenalineCoroutine != null)
            StopCoroutine(adrenalineCoroutine);
        adrenalineCoroutine = StartCoroutine(AdrenalineRushRoutine()); // 아드레날린 러시 지속 시간 동안 효과 적용
    }

    IEnumerator AdrenalineRushRoutine()
    {
        Debug.Log("아드레날린 러시 활성화");
        // 임시 이동속도 보너스 적용
        float originalBonus = playerStats.runBonus.moveSpeed;
        float boostAmount = playerStats.baseData.moveSpeed * 0.5f; // 기본 이동속도의 50% 추가

        playerStats.runBonus.moveSpeed += boostAmount;

        yield return new WaitForSeconds(2f); // 2초 동안 지속
        playerStats.runBonus.moveSpeed -= boostAmount; // 보너스 제거
        Debug.Log("아드레날린 러시 비활성화");
    }

    public bool CheckLuckySeven()
    {
        if (!hasLuckySeven) return false;

        float luckBonus =0.07f + (0.1f * playerStats.FinalLuck);
        if (Random.value < luckBonus)
        {
            Debug.Log("777 효과 발동! 치즈 3개 추가 획득!");
            return true;
        }
        return false;
    }
    public void ApplyCard(string cardName, float value)
    {
        switch (cardName)
        {
            case "Magnet": UseMagnet(value); break;
            case "SpecialMove": if (value >= 999f) SetSpecialMove(true, 0); else SetSpecialMove(false, (int)value); break;
            case "DisposableShield": AddShield((int)value); break;
            case "SharpFangs": ActivateSharpFangs(); break;
            case "SlowMotion": ActivateSlowMotion(value); break;
            case "AdrenalineRush": hasAdrenalineRush = true; break;
            case "DoT": hasDoT = true; break;
            case "RapidStrike": hasRapidStrike = true; break;
            case "LuckySeven": hasLuckySeven = true; break;
            case "DealWithDevil": hasDealWithDevil = true; break;
        }
    }
}
