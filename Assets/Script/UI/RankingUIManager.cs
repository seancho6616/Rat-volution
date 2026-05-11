using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 서버에서 받아올 랭킹 데이터 구조
[System.Serializable]
public struct RankingData
{
    public int rank;
    public string userName;
    public int score;
}

public class RankingUIManager : MonoBehaviour
{
    [Header("랭킹 UI 연결")]
    public Button backBtn;
    public Button rankingBtn;
    public GameObject ranking;

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
        if(rankingBtn != null) rankingBtn.onClick.AddListener(OnRankingBtnClick);
        if(backBtn != null) backBtn.onClick.AddListener(OnBackBtnClick);
        

        if(ranking != null) ranking.SetActive(false);
    }

    public void OnRankingBtnClick()
    {
        ranking.SetActive(true);
        inventory.SetActive(false);

        // 랭킹 창이 열릴 때 리스트를 생성/갱신
        RefreshRankingList();
    }

    public void OnBackBtnClick()
    {
        ranking.SetActive(false);
        inventory.SetActive(true);
    }

    public void RefreshRankingList()
    {
        // 1. 기존 리스트 청소
        foreach (Transform child in rankingContent)
        {
            Destroy(child.gameObject);
        }

        // 2. 서버에서 랭킹 데이터 받아오기 (현재는 더미 데이터)
        // 추후 ApiManager.instance.GetRanking(...) 같은 형태로 바뀔 부분입니다.
        List<RankingData> rankList = GetDummyRankingData();

        // ★ 3. 리스트 생성 및 데이터 주입 (최대 100명 제한)
        // 데이터가 100개 미만일 때는 데이터 개수만큼만, 100개가 넘으면 100까지만 반복
        int displayCount = Mathf.Min(rankList.Count, 100); 

        for (int i = 0; i < displayCount; i++)
        {
            var data = rankList[i]; // 순서대로 데이터 꺼내오기
            
            GameObject newRow = Instantiate(rankingInfoBarPrefab, rankingContent);
            
            // 프리팹 전용 스크립트를 가져와서 데이터 세팅
            RankContent rowScript = newRow.GetComponent<RankContent>();
            if (rowScript != null)
            {
                rowScript.SetData(data.rank, data.userName, data.score);
            }
        }

        // 4. 내 기록 세팅
        if (myRank != null) myRank.text = "-"; // 임시 순위
        if (myNickname != null) 
        {
            // GameManager에 닉네임이 있으면 띄우고, 없으면 "게스트"로 표기
            myNickname.text = !string.IsNullOrEmpty(GameManager.instance?.nickname) 
                                 ? GameManager.instance.nickname 
                                 : "게스트";
        }
        if (myRecord != null) myRecord.text = "0"; // 임시 점수
    }

    // 서버 통신 전 테스트용 데이터
    private List<RankingData> GetDummyRankingData()
    {
        List<RankingData> list = new List<RankingData>();
        for (int i = 1; i <= 20; i++) 
        {
            list.Add(new RankingData { rank = i, userName = "Player_" + i, score = 5000 - (i * 100) });
        }
        return list;
    }

}
