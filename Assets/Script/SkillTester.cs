using UnityEngine;

public partial class SkillTester : MonoBehaviour
{
    private PlayerSkill skill;

    void Start() => skill = GetComponent<PlayerSkill>();

    void Update()
    {
        // 1번 누르면 자석 발동
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("테스트: 자석(Magnet) 발동!");
            skill.ApplyCard("Magnet", 15f); // 범위 15
        }

        // 2번 누르면 벽 뛰어넘기 활성화
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("테스트: 특수 이동(Wall Jump) 활성화!");
            skill.ApplyCard("SpecialMove", 1f);
        }

        // 3번 누르면 보호막 1개 추가
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("테스트: 보호막(Shield) 1개 추가!");
            skill.ApplyCard("Shield", 1f);
        }
    }
}