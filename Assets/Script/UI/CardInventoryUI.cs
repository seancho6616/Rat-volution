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

    // 임시 데이터
    private BaseCardData dummyData;

    void Awake()
    {
        if(Instance == null) Instance = this;
    }

    void Start()
    {
        // 버튼 클릭 이벤트 연결
        if(prevBtn != null) prevBtn.onClick.AddListener(OnPrevClick);
        if(nextBtn != null) nextBtn.onClick.AddListener(OnNextClick);
        if(popupCloseBtn != null) popupCloseBtn.onClick.AddListener(CloseCardPopup);
        if(BackBtn != null) BackBtn.onClick.AddListener(OnBackBtnClick);
        if(inventoryBtn != null)inventoryBtn.onClick.AddListener(OpenInventory);

        // 시작 시 팝업, 도감창은 숨겨둠
        if(cardPopupPanel != null) cardPopupPanel.SetActive(false);
        if(cardInventory != null) cardInventory.SetActive(false);
    }

    public void OpenInventory()
    {
        currentPage = 0;
        cardInventory.SetActive(true);
        UpdateUI();
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

            // [추후 작업] 실제 획득 여부를 검사하는 로직이 들어갈 자리입니다.
            // 예: bool isDiscovered = PlayerInventory.HasCard(actualData.code);
            // 지금은 테스트를 위해 모두 획득(true) 상태이거나, 특정 조건으로 보이게 합니다.
            bool isDiscovered = true; // 전부 앞면으로 보이게 세팅 (테스트용)
            
            if(cardScript != null)
            {
                // 꺼내온 실제 데이터를 카드 UI에 덮어씌웁니다.
                cardScript.SetCardData(actualData, CardUI.CardMode.Inventory, isDiscovered);
            }
        }

        // 3. 버튼 활성화/비활성화 처리
        if(prevBtn != null) prevBtn.gameObject.SetActive(currentPage > 0);
        if(nextBtn != null) nextBtn.gameObject.SetActive((currentPage + 1) * CardsPerSpread < totalCards);
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
        // 다음 페이지 로직
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
            cardPopupPanel.SetActive(true); // 팝업 띄우기
            // 팝업 카드는 Popup 모드로 켬 (클릭하면 뒤집어짐)
            popupCardUI.SetCardData(cardData, CardUI.CardMode.Popup, true);
        }
    }

    public void CloseCardPopup()
    {
        if (cardPopupPanel != null) cardPopupPanel.SetActive(false);
    }
}