using UnityEngine;

/*  카드 종합 데이터
카드 타입별 Data 파일 삽입
카드타입과 카드 등급은 앞에서부터 0,1,2...
CardType    - 카드 종류 선언
CardRarity  - 카드 등급 선언
icon은 인벤토리 UI에 동일 사용
*/
public enum CardType{ StatUp, Item, Debuff }
public enum CardRarity{Common, Epic, Legend, Debuff }
[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    [Header("공통 정보")]
    public string       cardName;       // 카드 이름
    public string       description;    // 카드 설명
    public CardType     cardType;       // 카드 종류
    public CardRarity   cardRarity;     // 카드 등급
    
    [Header("카드 종류별 데이터")]
    public StatCardData     statCardData;   // 스텟 데이터
    public ItemCardData     itemcardData;   // 아이템 데이터
    public DebuffCardData   debuffCardData; // 디버프 데이터
    
    [Header("카드 아이콘")]
    public Sprite       icon;
}
