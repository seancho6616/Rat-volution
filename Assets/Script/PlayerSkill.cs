using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSkill : MonoBehaviour
{
    private PlayerControl playerControl;

    [Header("Active Skills")]
    public bool canJumpOverWall = false;
    public int shieldCount = 0;

    [Header("Magnet Settings")]
    public float magnetRange = 10f;

    // 벽 넘기 스킬 사용 여부
    public void ActiveSpecialMove()
    {
        canJumpOverWall = true;
        Debug.Log("벽 넘기 스킬 활성화");
    }

    // 치즈 당겨오기
    public void UseMagnet()
    {
        // 1칸 범위 내 치즈 탐색 후 플레이어 위치로 이동
        Debug.Log("자석 스킬 사용");
    }

    // [날카로운 앞니] 확률 계산
    public bool CheckSharpTeeth()
    {
        // 20% 확률로 발동
        return Random.value < 0.2f;
    }
}
