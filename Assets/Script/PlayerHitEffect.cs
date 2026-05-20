using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHitEffect : MonoBehaviour
{
    [Header("효과 설정")]
    [SerializeField] private float duration = 1.5f; // 총 지속 시간
    [SerializeField] private float flickerSpeed = 0.1f; // 깜빡임 속도
    
    // 맞았을 때 반짝일 색상 (인스펙터에서 하얀색이나 빨간색으로 설정해 보세요!)
    [SerializeField] private Color hitColor = Color.red; 

    private List<Renderer> renderers = new List<Renderer>(); 
    private List<Color> originalColors = new List<Color>(); 
    private bool isEffectRunning = false;

    private void Start()
    {
        // 모든 자식의 렌더러와 원래 색상을 미리 저장해둡니다.
        // renderers.AddRange(GetComponentsInChildren<Renderer>());
        // foreach (var r in renderers)
        // {
        //     // URP Lit 셰이더는 기본 색상 변수명이 "_BaseColor"입니다.
        //     originalColors.Add(r.material.GetColor("_BaseColor"));
        // }
    }

    public void PlayHitEffect()
    {
        if (isEffectRunning) return;
        StartCoroutine(HitColorRoutine());
    }

    private IEnumerator HitColorRoutine()
    {
        isEffectRunning = true;
        Debug.Log("<color=red>[Player] 피격! 색상 깜빡임 시작</color>");

        float timer = 0f;
        while (timer < duration)
        {
            // 1. 모든 부위를 '피격 색상'으로 바꿉니다. (Opaque여도 아주 잘 보임!)
            SetColor(hitColor);
            yield return new WaitForSeconds(flickerSpeed);
            timer += flickerSpeed;

            // 2. 다시 '원래 색상'으로 돌립니다.
            SetColor(originalColors);
            yield return new WaitForSeconds(flickerSpeed);
            timer += flickerSpeed;
        }

        // 마지막에는 무조건 원래 색상으로 복구
        SetColor(originalColors);
        isEffectRunning = false;
    }

    // 전체 색상 변경 함수
    private void SetColor(Color color)
    {
        foreach (var r in renderers)
        {
            if (r != null) r.material.SetColor("_BaseColor", color);
        }
    }

    // 리스트에 저장된 개별 원본 색상으로 복구하는 함수
    private void SetColor(List<Color> colors)
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            if (renderers[i] != null) renderers[i].material.SetColor("_BaseColor", colors[i]);
        }
    }
}