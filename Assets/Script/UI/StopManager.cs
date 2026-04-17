using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject exitWindow;

    void Start()
    {
        // 게임이 처음 시작될 때는 종료 창이 보이지 않도록 비활성화
        if (exitWindow != null)
        {
            exitWindow.SetActive(false);
        }
    }

    // 우측 상단 'Stop BT'를 눌렀을 때 실행될 함수
    public void OpenExitWindow()
    {
        exitWindow.SetActive(true);  // 창 띄우기
        Time.timeScale = 0f;         // 게임 진행 멈추기 (선택 사항: 배경 게임을 멈추고 싶을 때)
    }

    // 'NO' 버튼을 눌렀을 때 실행될 함수
    public void ContinueGame()
    {
        exitWindow.SetActive(false); // 창 숨기기
        Time.timeScale = 1f;         // 게임 다시 진행하기
    }

    // 'YES' 버튼을 눌렀을 때 실행될 함수
    public void QuitGame()
    {
        // Unity 에디터 내에서는 Application.Quit()이 작동하지 않으므로 확인용 로그를 띄웁니다.
        Debug.Log("게임이 종료되었습니다!"); 
        
        // 실제 빌드된 게임(exe, apk 등)을 종료하는 코드
        Application.Quit(); 

        // 에디터 내에서 플레이 종료
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}