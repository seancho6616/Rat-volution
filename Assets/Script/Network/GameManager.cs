using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public string userId;
    public string nickname;
    public string gameRunId;

    // 이번 판에 획득한 카드 code (게임 종료 시 서버로 전송)
    public List<string> discoveredCards = new List<string>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 게임 시작 시 호출 - 이전 판 데이터 초기화
    public void ResetRunData()
    {
        discoveredCards.Clear();
    }

    // 카드 획득 시 호출 - ItemManager 쪽에서 부를 예정
    public void AddDiscoveredCard(string code)
    {
        if (!string.IsNullOrEmpty(code) && !discoveredCards.Contains(code))
        {
            discoveredCards.Add(code);
        }
    }
}