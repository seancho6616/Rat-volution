using UnityEngine;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 따라갈 대상 (플레이어)
    public Vector3 offset; // 카메라와 대상 사이의 오프셋
    public float smoothSpeed = 0.125f; // 카메라 이동의 부드러움 정도

    [Header("Wall Transparency")]
    public LayerMask wallLayer; // 벽 레이어
    private List<Renderer> obscuredRenderers = new List<Renderer>(); // 가려진 벽의 렌더러 리스트

    void LateUpdate()
    {
        if (target == null) return;
        // 카메라 위치 업데이트
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 가려진 벽 감지
        HandleWallTransparency();
    }

    void HandleWallTransparency()
    {
        foreach (var renderer in obscuredRenderers)
        {
            if (renderer != null)
            {
                SetWallAlpha(renderer, 1f); // 원래 투명도로 복원
            }
        }
        obscuredRenderers.Clear();

        // 카메라와 플레이어 사이의 벽 감지
        Vector3 direction = (target.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target.position);

        // Raycast를 사용하여 벽 감지
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, wallLayer);

        foreach (var hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                SetWallAlpha(renderer, 0.3f); // 투명도 설정
                obscuredRenderers.Add(renderer);
            }
        }
    }

    void SetWallAlpha(Renderer renderer, float alpha)
    {
        Color color = renderer.material.GetColor("_BaseColor");
        color.a = alpha;
        renderer.material.SetColor("_BaseColor", color);

        if (alpha < 1f)
        {
            renderer.material.SetInt("_ZWrite", 1);
            renderer.material.renderQueue = 3000; // 투명 렌더링 큐
        }
        else
        {
            renderer.material.SetInt("_ZWrite", 1);
            renderer.material.renderQueue = -1; // 기본 렌더링 큐
        }
    }
}
