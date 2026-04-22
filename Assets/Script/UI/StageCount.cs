using UnityEngine;
using TMPro;
using System.Collections;

public class StageCount : MonoBehaviour
{
    public TextMeshProUGUI stageText;

    void Start()
    {
        UpdateStageUI();
        StartCoroutine(StartGameRun());
    }

    // 서버에 게임 시작 알림 (API-GAM-001 / REQ-043)
    private IEnumerator StartGameRun()
    {
        // 방어: 로그인/게스트 미연결 상태면 API 호출 스킵
        if (GameManager.instance == null || string.IsNullOrEmpty(GameManager.instance.userId))
        {
            Debug.LogWarning("[StageCount] userId 없음 - GameStart API 호출 건너뜀");
            yield break;
        }

        // 방어: ApiManager 미존재 시
        if (ApiManager.instance == null)
        {
            Debug.LogError("[StageCount] ApiManager.instance가 null - 씬에 ApiManager 오브젝트가 있는지 확인");
            yield break;
        }

        yield return StartCoroutine(ApiManager.instance.GameStart(
            onSuccess: () => Debug.Log("[StageCount] game_run_id 발급 완료: " + GameManager.instance.gameRunId),
            onFail: (error) => Debug.LogError("[StageCount] GameStart 실패: " + error)
        ));
    }

    // 아이템을 먹었을 때 호출될 함수
    public void OnItemCollected()
    {
        UpdateStageUI();
    }

    public void UpdateStageUI()
    {
        if (PlayerStats.Instance != null && stageText != null)
        {
            stageText.text = "WAVE : " + PlayerStats.Instance.level;
        }
    }
}