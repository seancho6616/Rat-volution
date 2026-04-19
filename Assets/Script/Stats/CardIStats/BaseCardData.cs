using UnityEngine;

/*  기본 카드 데이터
카드 타입별 Data 파일 삽입
카드타입과 카드 등급은 앞에서부터 0,1,2...
CardType    - 카드 종류 선언
CardRarity  - 카드 등급 선언
icon은 인벤토리 UI에 동일 사용
*/
public enum CardType{ StatUp, Item, Debuff }
public enum CardRarity{Common, Epic, Legend, Debuff }
public class BaseCardData : ScriptableObject
{
    [Header("공통 정보")]
    public string       cardName;       // 카드 이름
    public string       description;    // 카드 설명
    public CardType     cardType;       // 카드 종류
    public CardRarity   cardRarity;     // 카드 등급
    public float        amount;         // 카드 수치 값
    
    [Header("카드 아이콘")]
    public Sprite       icon;
}
