using System.Collections;
using UnityEngine;

public class Wall_HitFlashEffect : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    private static readonly int BaseMapID = Shader.PropertyToID("_BaseMap");
    // URP/Lit 셰이더는 "Base Map"의 컬러를 _BaseColor로 제어합니다
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    private Color originalColor;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();

        originalColor = meshRenderer.material.GetColor(BaseColorID);
    }

    public void Flash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        meshRenderer.material.SetColor(BaseColorID, flashColor);
        yield return new WaitForSeconds(flashDuration);
        meshRenderer.material.SetColor(BaseColorID, originalColor);
        flashCoroutine = null;
    }
}
