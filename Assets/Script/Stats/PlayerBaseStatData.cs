using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBaseStatData", menuName = "Scriptable Objects/PlayerBaseStatData")]
public class PlayerBaseStatData : ScriptableObject
{
    [Header("생존")]
    public int      maxHP           = 3;    //최대 체력
    public int      wallCount       = 6;    // 벽 부술 수 있는 개수
    [Header("이동")]
    public float    moveSpeed       = 0.8f; // 이동속도
    [Header("공격")]
    public float    attackSpeed     = 0.4f; // 공격속도
    public float    objectAttack    = 1f;   // 오브젝트 공격
    public float    wallAttack      = 2f;   // 벽 공격
    [Header("기타")]
    public float    luck            = 0.1f; // 행운
    public float    insight         = 0f;   // 통찰력
}
