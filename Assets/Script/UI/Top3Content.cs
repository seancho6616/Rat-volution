using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Top3Content : MonoBehaviour
{
    [Header("UI 연결")]
    public Image medalImg;      // 1, 2, 3등 메달 이미지
    public TMP_Text usernameTxt; // 유저 닉네임
    public TMP_Text recordTxt;   // 점수/기록

    // 데이터를 채워주는 함수
    public void SetAlphaData(Sprite medal, string name, int score)
    {
        if (medalImg != null) medalImg.sprite = medal;
        if (usernameTxt != null) usernameTxt.text = name;
        if (recordTxt != null) recordTxt.text = score.ToString();
    }
}
