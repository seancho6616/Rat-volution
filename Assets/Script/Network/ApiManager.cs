using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ApiManager : MonoBehaviour
{
    public static ApiManager instance;

    // private string baseUrl = "http://localhost:3000"; 개발용
    private string baseUrl = "https://rat-volutionbackend-production.up.railway.app"; //배포용

    // JWT 토큰 (메모리 + PlayerPrefs 양쪽에 보관)
    private string authToken = "";

    // PlayerPrefs 키
    private const string PREF_TOKEN = "auth_token";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // 시작 시 저장된 토큰 로드
            authToken = PlayerPrefs.GetString(PREF_TOKEN, "");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ============================================================
    // 요청/응답 데이터 구조
    // ============================================================

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
        public string token;
    }

    [System.Serializable]
    public class SessionResponse
    {
        public string message;
        public string user_id;
        public string nickname;
        public bool is_guest;
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
        public string[] discovered_cards;
    }

    // 도감 응답 데이터
    [System.Serializable]
    public class DexCard
    {
        public string code;
        public string name;
        public string type;
        public string item_type;
        public string rarity;
        public string description;
        public float base_value;
        public float scale_per_stack;
        public int max_stack;
        public bool discovered;
    }

    [System.Serializable]
    public class DexResponse
    {
        public string message;
        public string nickname;
        public int total_cards;
        public int discovered_count;
        public DexCard[] cards;
    }

    // 랭킹 응답 데이터
    [System.Serializable]
    public class RankingEntry
    {
        public int rank;
        public string user_id;
        public string nickname;
        public int max_wave_reached;
        public int total_cheese;
        public string achieved_at;
    }

    [System.Serializable]
    public class RankingResponse
    {
        public string message;
        public RankingEntry[] leaderboard;
    }

    [System.Serializable]
    public class MyRankingResponse
    {
        public string message;
        public int rank;
        public string nickname;
        public int max_wave_reached;
        public int total_cheese;
        public string achieved_at;
    }

    // ============================================================
    // 토큰 관리
    // ============================================================

    public bool HasToken()
    {
        return !string.IsNullOrEmpty(authToken);
    }

    private void SaveToken(string token)
    {
        authToken = token;
        PlayerPrefs.SetString(PREF_TOKEN, token);
        PlayerPrefs.Save();
    }

    public void ClearToken()
    {
        authToken = "";
        PlayerPrefs.DeleteKey(PREF_TOKEN);
        PlayerPrefs.Save();
    }

    // 인증 정보 저장 (로그인/회원가입/게스트 응답 처리)
    private void SaveAuth(LoginResponse response)
    {
        SaveToken(response.token);
        if (GameManager.instance != null)
        {
            GameManager.instance.userId = response.user_id;
            GameManager.instance.nickname = response.nickname;
        }
    }

    // ============================================================
    // 인증 API
    // ============================================================

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
            useAuth: false,
            onSuccess: (result) =>
            {
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(result);
                SaveAuth(response);
                onSuccess?.Invoke();
            },
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
            useAuth: false,
            onSuccess: (result) =>
            {
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(result);
                SaveAuth(response);
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
            useAuth: false,
            onSuccess: (result) =>
            {
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(result);
                SaveAuth(response);
                onSuccess?.Invoke();
            },
            onFail: (error) => onFail?.Invoke(error)
        ));
    }

    // 세션 복원 GET /auth/session
    public IEnumerator RestoreSession(System.Action onSuccess, System.Action<string> onFail)
    {
        if (!HasToken())
        {
            onFail?.Invoke("저장된 토큰 없음");
            yield break;
        }

        yield return StartCoroutine(Get("/auth/session",
            useAuth: true,
            onSuccess: (result) =>
            {
                SessionResponse response = JsonUtility.FromJson<SessionResponse>(result);
                if (GameManager.instance != null)
                {
                    GameManager.instance.userId = response.user_id;
                    GameManager.instance.nickname = response.nickname;
                }
                Debug.Log("[Auth] 세션 복원 성공: " + response.nickname);
                onSuccess?.Invoke();
            },
            onFail: (error) =>
            {
                Debug.LogWarning("[Auth] 세션 복원 실패 - 토큰 만료 또는 무효: " + error);
                ClearToken();
                onFail?.Invoke(error);
            }
        ));
    }

    // ============================================================
    // 게임 API (인증 필요)
    // ============================================================

    public IEnumerator GameStart(System.Action onSuccess = null, System.Action<string> onFail = null)
    {
        yield return StartCoroutine(Post("/game/start", "{}",
            useAuth: true,
            onSuccess: (result) =>
            {
                GameStartResponse response = JsonUtility.FromJson<GameStartResponse>(result);
                GameManager.instance.gameRunId = response.game_run_id;
                GameManager.instance.ResetRunData();
                Debug.Log("게임 시작: " + response.game_run_id);
                onSuccess?.Invoke();
            },
            onFail: (error) =>
            {
                Debug.LogError("게임 시작 실패: " + error);
                onFail?.Invoke(error);
            }
        ));
    }

    public IEnumerator GameEnd(string status, int final_wave, int total_cheese, int final_hp, Stats stats,
        System.Action onSuccess = null, System.Action<string> onFail = null)
    {
        GameEndRequest data = new GameEndRequest
        {
            game_run_id = GameManager.instance.gameRunId,
            status = status,
            final_wave = final_wave,
            total_cheese_earned = total_cheese,
            final_hp = final_hp,
            stats = stats,
            discovered_cards = GameManager.instance.discoveredCards.ToArray()
        };

        yield return StartCoroutine(Post("/game/end", JsonUtility.ToJson(data),
            useAuth: true,
            onSuccess: (result) =>
            {
                Debug.Log("게임 종료 저장 완료");
                onSuccess?.Invoke();
            },
            onFail: (error) =>
            {
                Debug.LogError("게임 종료 실패: " + error);
                onFail?.Invoke(error);
            }
        ));
    }

    // ============================================================
    // 도감 API (인증 필요)
    // ============================================================

    public IEnumerator GetDex(System.Action<DexResponse> onSuccess, System.Action<string> onFail)
    {
        if (!HasToken())
        {
            onFail?.Invoke("로그인이 필요합니다");
            yield break;
        }

        yield return StartCoroutine(Get("/card/dex",
            useAuth: true,
            onSuccess: (result) =>
            {
                DexResponse response = JsonUtility.FromJson<DexResponse>(result);
                onSuccess?.Invoke(response);
            },
            onFail: (error) =>
            {
                Debug.LogError("도감 조회 실패: " + error);
                onFail?.Invoke(error);
            }
        ));
    }

    // ============================================================
    // 랭킹 API
    // ============================================================

    // TOP 100 랭킹 GET /leaderboard (인증 불필요)
    public IEnumerator GetRanking(System.Action<RankingResponse> onSuccess, System.Action<string> onFail)
    {
        yield return StartCoroutine(Get("/leaderboard",
            useAuth: false,
            onSuccess: (result) =>
            {
                RankingResponse response = JsonUtility.FromJson<RankingResponse>(result);
                onSuccess?.Invoke(response);
            },
            onFail: (error) =>
            {
                Debug.LogError("랭킹 조회 실패: " + error);
                onFail?.Invoke(error);
            }
        ));
    }

    // 내 랭킹 GET /leaderboard/me (인증 필요)
    public IEnumerator GetMyRanking(System.Action<MyRankingResponse> onSuccess, System.Action<string> onFail)
    {
        if (!HasToken())
        {
            onFail?.Invoke("로그인이 필요합니다");
            yield break;
        }

        yield return StartCoroutine(Get("/leaderboard/me",
            useAuth: true,
            onSuccess: (result) =>
            {
                MyRankingResponse response = JsonUtility.FromJson<MyRankingResponse>(result);
                onSuccess?.Invoke(response);
            },
            onFail: (error) =>
            {
                // 404일 수 있음 (랭킹 기록 없는 신규 유저). 정상 케이스라 LogWarning만.
                Debug.LogWarning("내 랭킹 조회 실패: " + error);
                onFail?.Invoke(error);
            }
        ));
    }

    // ============================================================
    // 공통 HTTP 메서드
    // ============================================================

    private IEnumerator Post(string endpoint, string json, bool useAuth,
        System.Action<string> onSuccess, System.Action<string> onFail)
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(baseUrl + endpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        if (useAuth && !string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader("Authorization", "Bearer " + authToken);
        }

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

    private IEnumerator Get(string endpoint, bool useAuth,
        System.Action<string> onSuccess, System.Action<string> onFail)
    {
        UnityWebRequest request = UnityWebRequest.Get(baseUrl + endpoint);

        if (useAuth && !string.IsNullOrEmpty(authToken))
        {
            request.SetRequestHeader("Authorization", "Bearer " + authToken);
        }

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
}