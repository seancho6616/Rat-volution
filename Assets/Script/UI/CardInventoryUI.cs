using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInventoryUI : MonoBehaviour
{
    public static CardInventoryUI Instance;

    [Header("실제 카드 데이터 DB")]
    public List<BaseCardData> allCardDatabase;

    [Header("도감 UI 연결")]
    public GameObject cardPrefab;      // 만들어둔 Card_Base_Prefab
    public Transform leftPageGrid;     // LeftPageGrid
    public Transform rightPageGrid;    // RightPageGrid
    public Button prevBtn;             // 이전 페이지 버튼
    public Button nextBtn;             // 다음 페이지 버튼
    public Button BackBtn;
    public GameObject cardInventory;
    public Button inventoryBtn;

    [Header("팝업 UI 연결")]
    public GameObject cardPopupPanel;  // 검은색 반투명 팝업 배경 판
    public CardUI popupCardUI;         // 팝업 화면 한가운데 있을 거대 카드
    public Button popupCloseBtn;       // 닫기 버튼 (X 표시 등)

    private int currentPage = 0;
    private const int CardsPerSpread = 8; // 양쪽 페이지 합쳐서 총 8장 (4+4)
    private List<GameObject> spawnedCards = new List<GameObject>(); // 생성된 카드 관리용

    // 서버에서 가져온 획득 카드 code 집합 (REQ-038, REQ-047)
    private HashSet<string> discoveredCodes = new HashSet<string>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 버튼 클릭 이벤트 연결
        if (prevBtn != null) prevBtn.onClick.AddListener(OnPrevClick);
        if (nextBtn != null) nextBtn.onClick.AddListener(OnNextClick);
        if (popupCloseBtn != null) popupCloseBtn.onClick.AddListener(CloseCardPopup);
        if (BackBtn != null) BackBtn.onClick.AddListener(OnBackBtnClick);
        if (inventoryBtn != null) inventoryBtn.onClick.AddListener(OpenInventory);

        // 시작 시 팝업, 도감창은 숨겨둠
        if (cardPopupPanel != null) cardPopupPanel.SetActive(false);
        if (cardInventory != null) cardInventory.SetActive(false);
    }

    public void OpenInventory()
    {
        currentPage = 0;
        cardInventory.SetActive(true);

        // 서버에서 도감 데이터 가져온 후 UI 갱신
        StartCoroutine(LoadAndShow());
    }

    private IEnumerator LoadAndShow()
    {
        // 게스트/로그인 안 된 경우 로컬 데이터로만 표시
        if (ApiManager.instance == null || GameManager.instance == null
            || string.IsNullOrEmpty(GameManager.instance.userId))
        {
            Debug.LogWarning("[Dex] 로그인 정보 없음 - 미획득 상태로 표시");
            discoveredCodes.Clear();
            UpdateUI();
            yield break;
        }

        yield return StartCoroutine(ApiManager.instance.GetDex(
            onSuccess: (response) =>
            {
                discoveredCodes.Clear();
                foreach (var card in response.cards)
                {
                    if (card.discovered)
                        discoveredCodes.Add(card.code);
                }
                Debug.Log($"[Dex] 획득 카드 {discoveredCodes.Count}장 로드 완료");
                UpdateUI();
            },
            onFail: (error) =>
            {
                Debug.LogError("[Dex] 도감 조회 실패: " + error);
                discoveredCodes.Clear();
                UpdateUI();   // 실패해도 미획득 상태로라도 표시
            }
        ));
    }

    private void UpdateUI()
    {
        // 1. 기존에 생성된 카드들 싹 지우기 (초기화)
        foreach (var card in spawnedCards) Destroy(card);
        spawnedCards.Clear();

        // 실제 연결된 카드 DB의 총 개수를 가져옴
        int totalCards = allCardDatabase.Count;
        int startIndex = currentPage * CardsPerSpread;

        for (int i = 0; i < CardsPerSpread; i++)
        {
            int cardIndex = startIndex + i;
            if (cardIndex >= totalCards) break;

            Transform parentGrid = (i < CardsPerSpread / 2) ? leftPageGrid : rightPageGrid;
            GameObject newCard = Instantiate(cardPrefab, parentGrid);
            spawnedCards.Add(newCard);

            CardUI cardScript = newCard.GetComponent<CardUI>();

            // DB 리스트에서 순서대로 실제 카드 데이터를 꺼내옴
            BaseCardData actualData = allCardDatabase[cardIndex];

            // 실제 획득 여부 검사 (서버 응답 기반)
            bool isDiscovered = !string.IsNullOrEmpty(actualData.code)
                                && discoveredCodes.Contains(actualData.code);

            if (cardScript != null)
            {
                cardScript.SetCardData(actualData, null, CardUI.CardMode.Inventory, isDiscovered);
            }
        }

        // 3. 버튼 활성화/비활성화 처리
        if (prevBtn != null) prevBtn.gameObject.SetActive(currentPage > 0);
        if (nextBtn != null) nextBtn.gameObject.SetActive((currentPage + 1) * CardsPerSpread < totalCards);
    }

    private void OnPrevClick()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateUI();
        }
    }

    private void OnNextClick()
    {
        currentPage++;
        UpdateUI();
    }

    public void OnBackBtnClick()
    {
        cardInventory.SetActive(false);
    }

    //  도감에서 획득한 카드 클릭 시 호출됨
    public void OpenCardPopup(BaseCardData cardData)
    {
        if (cardPopupPanel != null && popupCardUI != null)
        {
            cardPopupPanel.SetActive(true);
            popupCardUI.SetCardData(cardData, null, CardUI.CardMode.Popup, true);
        }
    }

    public void CloseCardPopup()
    {
        if (cardPopupPanel != null) cardPopupPanel.SetActive(false);
    }
}