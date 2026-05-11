using UnityEngine;
using TMPro;

public class RankContent : MonoBehaviour
{
    [Header("UI 연결")]
    public TMP_Text userRankTxt;
    public TMP_Text userNicknameTxt;
    public TMP_Text userRecordTxt;

    // 매니저에서 이 함수를 부르며 데이터를 던져줌
    public void SetData(int rank, string nickname, int score)
    {
        if (userRankTxt != null) userRankTxt.text = rank.ToString();
        if (userNicknameTxt != null) userNicknameTxt.text = nickname;
        if (userRecordTxt != null) userRecordTxt.text = score.ToString();
    }
}
