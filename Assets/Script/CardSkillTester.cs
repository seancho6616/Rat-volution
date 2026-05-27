using UnityEngine;

public class CardSkillTester : MonoBehaviour
{
    private PlayerSkill playerSkill;

    void Awake()
    {
        // 플레이어에게 붙어있는 PlayerSkill 컴포넌트를 가져옵니다.
        playerSkill = GetComponent<PlayerSkill>();
    }

    void Update()
    {
        // 싱글톤 인스턴스들이 비어있다면 에러 방지를 위해 리턴합니다.
        if (PlayerStats.Instance == null || PlayerStats.Instance.item == null) return;

        // [백스페이스 키] : 모든 인벤토리 아이템 및 스킬 상태 초기화
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            PlayerStats.Instance.item.Reset();
            // Debug.Log("<color=white><b>[치트] 인벤토리 스킬 수치 완전 초기화!</b></color>");
        }

        // [숫자 1] : 일회성 보호막 +1 (중첩 가능)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerStats.Instance.item.shield += 1f;
            // Debug.Log($"<color=cyan>[치트] 보호막 +1 주입 (현재 보호막 횟수: {PlayerStats.Instance.item.shield})</color>");
        }

        // [숫자 2] : 특수 이동(벽 넘기 점프) 해금 및 쿨타임 설정
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerStats.Instance.item.specialMove = 5f; // 쿨타임 5초 설정
            // Debug.Log($"<color=cyan>[치트] 벽 넘기 점프 스킬 해금! (쿨타임: {PlayerStats.Instance.item.specialMove}초)</color>");
        }

        // [숫자 3] : 자석 흡입 즉시 발동
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayerStats.Instance.item.magnet = true;
            if (playerSkill != null) playerSkill.UseMagnet();
            // Debug.Log("<color=cyan>[치트] 자석 스킬 보유 처리 및 주변 치즈 흡입 발동!</color>");
        }

        // [숫자 4] : 날카로운 앞니 확률 누적 (누를 때마다 20%씩 추가)
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayerStats.Instance.item.sharpFangs += 0.2f;
            // Debug.Log($"<color=cyan>[치트] 날카로운 앞니 확률 +20% 주입 (현재 인벤토리 값: {PlayerStats.Instance.item.sharpFangs * 100f}%)</color>");
        }

        // [숫자 5] : 슬로우 모션 즉시 발동 (★매개변수 오류 수정)
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayerStats.Instance.item.slowMotion = true;
            if (playerSkill != null) 
            {
                // 기획서 기준 사양인 20% 속도 감소를 위해 0.2f를 정확히 인자로 전달합니다.
                playerSkill.ActivateSlowMotion(0.2f); 
            }
            // Debug.Log("<color=cyan>[치트] 슬로우 모션 보유 처리 및 4초간 20% 감소 발동!</color>");
        }

        // [숫자 6] : 아드레날린 분비 즉시 발동
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            PlayerStats.Instance.item.adrenaline = true;
            if (playerSkill != null) playerSkill.TriggerAdrenalineRush();
            // Debug.Log("<color=cyan>[치트] 아드레날린 보유 처리 및 2초간 이동속도 50% 버프 발동!</color>");
        }

        // [숫자 7] : 도트뎀 퍼센트 누적 (누를 때마다 5%씩 추가)
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            PlayerStats.Instance.item.dot += 0.05f;
            // Debug.Log($"<color=cyan>[치트] 도트뎀 비율 +5% 주입 (현재 인벤토리 값: {PlayerStats.Instance.item.dot * 100f}%)</color>");
        }

        // [숫자 8] : 연속 공격 확률 누적 (누를 때마다 20%씩 추가)
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            PlayerStats.Instance.item.rapidStrike += 0.2f; 
            // Debug.Log($"<color=cyan>[치트] 연속 공격 확률 추가 주입 (현재 인벤토리 값: {PlayerStats.Instance.item.rapidStrike})</color>");
        }

        // [숫자 9] : 777 행운 보너스 확률 누적 (누를 때마다 7%씩 추가)
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            PlayerStats.Instance.item.luckySeven += 0.07f;
            // Debug.Log($"<color=cyan>[치트] 777 보너스 확률 +7% 주입 (현재 인벤토리 값: {PlayerStats.Instance.item.luckySeven * 100f}%)</color>");
        }

        // [숫자 0] : 악마와의 계약(부활 카드) 주입
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            PlayerStats.Instance.item.dealWithDevil = true;
            // Debug.Log("<color=cyan>[치트] 악마와의 계약(부활 카드) 주입 완료! 사망 시 작동합니다.</color>");
        }
    }
}