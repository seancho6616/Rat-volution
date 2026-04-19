using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public string userId;
    public string nickname;
    public string gameRunId;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환해도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }
}