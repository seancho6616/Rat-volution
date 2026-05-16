using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 서버에서 받아올 랭킹 데이터 구조 (UI 표시용)
[System.Serializable]
public struct RankingData
{
    public int rank;
    public string userName;
    public int score;
    public int totalCheese;
}

public class RankingUIManager : MonoBehaviour
{
    [Header("랭킹 UI 연결")]
    public Button backBtn;
    public Button rankingBtn;
    public GameObject ranking;

    [Header("TOP3 프리팹 설정")]
    public GameObject topRankPrefab;
    public Transform[] top3Positions;
    public Sprite[] medalSprites;

    [Header("랭킹 리스트 연결")]
    public GameObject rankingInfoBarPrefab;
    public Transform rankingContent;

    [Header("나의 랭킹 연결")]
    public TMP_Text myRank;
    public TMP_Text myNickname;
    public TMP_Text myRecord;

    [Header("Hide Panel")]
    public GameObject inventory;

    void Start()
    {
        if (rankingBtn != null) rankingBtn.onClick.AddListener(OnRankingBtnClick);
        if (backBtn != null) backBtn.onClick.AddListener(OnBackBtnClick);

        if (ranking != null) ranking.SetActive(false);
    }

    public void OnRankingBtnClick()
    {
        ranking.SetActive(true);
        inventory.SetActive(false);

        // 랭킹 창이 열릴 때 서버에서 데이터 받아오기
        StartCoroutine(LoadRankingFromServer());
    }

    public void OnBackBtnClick()
    {
        ranking.SetActive(false);
        inventory.SetActive(true);
    }

    // 서버에서 랭킹 데이터 받아오는 메인 흐름
    private IEnumerator LoadRankingFromServer()
    {
        // 기존 리스트 청소
        ClearRankingUI();

        // 1. TOP 100 받아오기
        List<RankingData> rankList = new List<RankingData>();

        yield return StartCoroutine(ApiManager.instance.GetRanking(
            onSuccess: (response) =>
            {
                if (response.leaderboard != null)
                {
                    foreach (var entry in response.leaderboard)
                    {
                        rankList.Add(new RankingData
                        {
                            rank = entry.rank,
                            userName = entry.nickname,
                            score = entry.max_wave_reached,
                            totalCheese = entry.total_cheese
                        });
                    }
                }
                Debug.Log($"[Ranking] TOP {rankList.Count}명 로드 완료");
            },
            onFail: (error) =>
            {
                Debug.LogError("[Ranking] 랭킹 조회 실패: " + error);
            }
        ));

        // 2. UI 렌더링
        RenderRankingList(rankList);

        // 3. 내 랭킹 받아오기
        yield return StartCoroutine(LoadMyRanking());
    }

    private void ClearRankingUI()
    {
        // 리스트 청소
        foreach (Transform child in rankingContent)
        {
            Destroy(child.gameObject);
        }
        // TOP3 청소
        for (int i = 0; i < top3Positions.Length; i++)
        {
            if (top3Positions[i] != null)
            {
                foreach (Transform child in top3Positions[i])
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    private void RenderRankingList(List<RankingData> rankList)
    {
        // TOP 3 표시
        int topCount = Mathf.Min(rankList.Count, 3);
        for (int i = 0; i < topCount; i++)
        {
            GameObject go = Instantiate(topRankPrefab, top3Positions[i]);

            RectTransform rect = go.GetComponent<RectTransform>();
            if (rect != null) rect.anchoredPosition = Vector2.zero;

            Top3Content script = go.GetComponent<Top3Content>();
            if (script != null)
            {
                script.SetAlphaData(medalSprites[i], rankList[i].userName, rankList[i].score);
            }
        }

        // 전체 리스트 표시 (최대 100명)
        int displayCount = Mathf.Min(rankList.Count, 100);

        for (int i = 0; i < displayCount; i++)
        {
            var data = rankList[i];

            GameObject newRow = Instantiate(rankingInfoBarPrefab, rankingContent);

            RankContent rowScript = newRow.GetComponent<RankContent>();
            if (rowScript != null)
            {
                rowScript.SetData(data.rank, data.userName, data.score);
            }
        }
    }

    // 내 랭킹 조회 및 표시
    private IEnumerator LoadMyRanking()
    {
        // 로그인 안 한 게스트 등은 스킵
        if (ApiManager.instance == null || !ApiManager.instance.HasToken())
        {
            SetMyRankingUI("-", "게스트", "0");
            yield break;
        }

        yield return StartCoroutine(ApiManager.instance.GetMyRanking(
            onSuccess: (response) =>
            {
                SetMyRankingUI(
                    response.rank.ToString(),
                    response.nickname,
                    response.max_wave_reached.ToString()
                );
                Debug.Log($"[Ranking] 내 랭킹: {response.rank}위");
            },
            onFail: (error) =>
            {
                // 404: 아직 게임 기록 없음 (신규 유저)
                string nickname = !string.IsNullOrEmpty(GameManager.instance?.nickname)
                    ? GameManager.instance.nickname
                    : "게스트";
                SetMyRankingUI("-", nickname, "0");
            }
        ));
    }

    private void SetMyRankingUI(string rank, string nickname, string record)
    {
        if (myRank != null) myRank.text = rank;
        if (myNickname != null) myNickname.text = nickname;
        if (myRecord != null) myRecord.text = record;
    }
}