using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ApiManager : MonoBehaviour
{
    public static ApiManager instance;

    private string baseUrl = "http://localhost:3000";

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

    // -------- 요청/응답 데이터 구조 --------

    [System.Serializable]
    public class RegisterRequest
    {
        public string login_id;
        public string nickname;
        public string password;
    }

    [System.Serializable]
    public class LoginRequest
    {
        public string login_id;
        public string password;
    }

    [System.Serializable]
    public class GuestRequest
    {
        public string uuid;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string message;
        public string user_id;
        public string nickname;
        public bool is_guest;
    }

    [System.Serializable]
    public class GameStartRequest
    {
        public string user_id;
    }

    [System.Serializable]
    public class GameStartResponse
    {
        public string message;
        public string game_run_id;
    }

    [System.Serializable]
    public class Stats
    {
        public float move_speed;
        public float luck;
        public float insight;
        public float attack_speed;
        public float power;
        public float attack_power;
    }

    [System.Serializable]
    public class GameEndRequest
    {
        public string game_run_id;
        public string status;
        public int final_wave;
        public int total_cheese_earned;
        public int final_hp;
        public Stats stats;
    }

    // -------- API 메서드 --------

    // 회원가입
    public IEnumerator Register(string login_id, string nickname, string password,
        System.Action onSuccess, System.Action<string> onFail)
    {
        RegisterRequest data = new RegisterRequest
        {
            login_id = login_id,
            nickname = nickname,
            password = password
        };

        yield return StartCoroutine(Post("/auth/register", JsonUtility.ToJson(data),
            onSuccess: (result) => onSuccess?.Invoke(),
            onFail: (error) => onFail?.Invoke(error)
        ));
    }

    // 로그인
    public IEnumerator Login(string login_id, string password,
        System.Action onSuccess, System.Action<string> onFail)
    {
        LoginRequest data = new LoginRequest
        {
            login_id = login_id,
            password = password
        };

        yield return StartCoroutine(Post("/auth/login", JsonUtility.ToJson(data),
            onSuccess: (result) =>
            {
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(result);
                GameManager.instance.userId = response.user_id;
                GameManager.instance.nickname = response.nickname;
                onSuccess?.Invoke();
            },
            onFail: (error) => onFail?.Invoke(error)
        ));
    }

    // 게스트 로그인
    public IEnumerator GuestLogin(System.Action onSuccess, System.Action<string> onFail)
    {
        string uuid = PlayerPrefs.GetString("guest_uuid", "");
        if (uuid == "")
        {
            uuid = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("guest_uuid", uuid);
        }

        GuestRequest data = new GuestRequest { uuid = uuid };

        yield return StartCoroutine(Post("/auth/guest", JsonUtility.ToJson(data),
            onSuccess: (result) =>
            {
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(result);
                GameManager.instance.userId = response.user_id;
                GameManager.instance.nickname = response.nickname;
                onSuccess?.Invoke();
            },
            onFail: (error) => onFail?.Invoke(error)
        ));
    }

    // 공통 POST (onFail 추가)
    private IEnumerator Post(string endpoint, string json,
        System.Action<string> onSuccess, System.Action<string> onFail)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(baseUrl + endpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(request.downloadHandler.text);
        }
        else
        {
            onFail?.Invoke(request.downloadHandler.text);
        }
    }    

    // 게임 시작
    public IEnumerator GameStart()
    {
        GameStartRequest data = new GameStartRequest
        {
            user_id = GameManager.instance.userId
        };

        yield return StartCoroutine(Post("/game/start", JsonUtility.ToJson(data), (result) =>
        {
            GameStartResponse response = JsonUtility.FromJson<GameStartResponse>(result);
            GameManager.instance.gameRunId = response.game_run_id;
            Debug.Log("게임 시작: " + response.game_run_id);
        }));
    }

    // 게임 종료
    public IEnumerator GameEnd(string status, int final_wave, int total_cheese, int final_hp, Stats stats)
    {
        GameEndRequest data = new GameEndRequest
        {
            game_run_id = GameManager.instance.gameRunId,
            status = status,
            final_wave = final_wave,
            total_cheese_earned = total_cheese,
            final_hp = final_hp,
            stats = stats
        };

        yield return StartCoroutine(Post("/game/end", JsonUtility.ToJson(data), (result) =>
        {
            Debug.Log("게임 종료 저장 완료");
        }));
    }

    // -------- 공통 POST 메서드 --------
    private IEnumerator Post(string endpoint, string json, System.Action<string> onSuccess)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(baseUrl + endpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccess(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError(endpoint + " 실패: " + request.downloadHandler.text);
        }
    }
}