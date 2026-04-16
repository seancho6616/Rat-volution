using UnityEngine;

/*  스탯 카드 데이터
StatType는 Stats-PlayerStats-PlayerStats에 있음
*/

[CreateAssetMenu(fileName = "StatCardData", menuName = "Scriptable Objects/StatCardData")]
public class StatCardData : ScriptableObject
{
    public StatType statType;   // 스탯 타입
    public float    amount;     // 증가 수치
}
