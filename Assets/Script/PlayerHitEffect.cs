using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitEffect : MonoBehaviour
{
    [Header("효과 설정")]
    [SerializeField] private float duration = 1.5f;        // 총 지속 시간
    [SerializeField] private float flickerSpeed = 0.1f;    // 깜빡임 속도
    [SerializeField] private Color hitColor = Color.red;   // 피격 시 색상

    [Header("렌더러 (비우면 자식에서 자동 탐색)")]
    [SerializeField] private Renderer[] renderers;

    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    private MaterialPropertyBlock propertyBlock;
    private Color[] originalColors;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();

        propertyBlock = new MaterialPropertyBlock();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].sharedMaterial.GetColor(BaseColorID);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();
    }
#endif

    public void PlayHitEffect()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(HitColorRoutine());
    }

    private IEnumerator HitColorRoutine()
    {
        Debug.Log("<color=red>[Player] 피격! 색상 깜빡임 시작</color>");

        float timer = 0f;
        var wait = new WaitForSeconds(flickerSpeed);

        while (timer < duration)
        {
            // 1. 피격 색상으로 변경
            SetColor(hitColor);
            yield return wait;
            timer += flickerSpeed;

            // 2. 원래 색상으로 복구
            RestoreOriginalColors();
            yield return wait;
            timer += flickerSpeed;
        }

        // 마지막에 무조건 원래 색상으로 복구
        RestoreOriginalColors();
        flashCoroutine = null;
    }

    // 모든 렌더러를 같은 색으로 설정
    private void SetColor(Color color)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;

            renderers[i].GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(BaseColorID, color);
            renderers[i].SetPropertyBlock(propertyBlock);
        }
    }

    // 각 렌더러를 자신의 원래 색으로 복구
    private void RestoreOriginalColors()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null) continue;

            renderers[i].GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(BaseColorID, originalColors[i]);
            renderers[i].SetPropertyBlock(propertyBlock);
        }
    }
}