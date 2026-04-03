using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHitEffect : MonoBehaviour
{
    [Header("효과 설정")]
    // 효과 지속 시간
    [SerializeField] private float duration = 1.5f;
    // 깜빡이는 속도
    [SerializeField] private float flickerSpeed = 0.1f;
    [Range(0f, 1f)]
    // 흐려졌을 때의 투명도
    [SerializeField] private float alphaValue = 0.3f;

    private List<Renderer> renderers = new List<Renderer>(); 
    
    // 효과 중복 실행 방지
    private bool isEffectRunning = false;

    private void Start()
    {
        renderers.AddRange(GetComponentsInChildren<Renderer>());
    }

    public void PlayHitEffect()
    {
        if (isEffectRunning) return;
        StartCoroutine(HitRoutine());
    }

    // 코루틴: 시간 차를 두고 투명도를 조절하는 핵심 로직
    private IEnumerator HitRoutine()
    {
        isEffectRunning = true;
        Debug.Log("[Player] 한 대 맞음");

        float timer = 0f;

        while (timer < duration)
        {
            SetAlpha(alphaValue);
            yield return new WaitForSeconds(flickerSpeed);
            timer += flickerSpeed;

            // B. 캐릭터를 다시 선명하게 만듭니다 (Alpha 값 1로 복구)
            SetAlpha(1.0f);
            yield return new WaitForSeconds(flickerSpeed);
            timer += flickerSpeed;
        }

        SetAlpha(1.0f);
        isEffectRunning = false;
    }

    // 캐릭터의 모든 부분의 투명도를 한 번에 바꾸는 헬퍼 함수
    private void SetAlpha(float alpha)
    {
        foreach (var renderer in renderers)
        {
            if (renderer.material == null) continue;

            Color color = renderer.material.color;
            color.a = alpha; // 투명도(a) 값 변경
            renderer.material.color = color;
        }
    }
}