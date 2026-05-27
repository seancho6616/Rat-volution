using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioClip eatSound;
    [Range(0f, 1f)] public float volume = 0.5f; // 볼륨 조절 슬라이더

    public int count = 1;
    [SerializeField] private Vector3 pos;
    
    void Update()
    {
        transform.Rotate(pos * Time.deltaTime, Space.World );
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            // --- 아이템 획득 사운드 재생 ---
            if (eatSound != null)
            {
                // PlayClipAtPoint를 사용해 파괴되어도 소리가 유지되도록 함
                // 카메라 위치에서 재생하면 거리에 따른 소리 감소 없이 선명하게 들려!
                if (Camera.main != null)
                {
                    AudioSource.PlayClipAtPoint(eatSound, Camera.main.transform.position, volume);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(eatSound, transform.position, volume);
                }
            }
            PlayerStats.Instance.GainCheese(count);
            // 1. 게이지 상승 (기존 로직 유지)
            Gauge gaugeScript = Object.FindAnyObjectByType<Gauge>();
            if (gaugeScript != null) gaugeScript.AddScore(1);

            // 2. 스테이지 체크 (수정된 부분: 바로 다음 스테이지로 가는 게 아니라 '하나 먹었다'고 신호만 보냄)
            StageCount stageScript = Object.FindAnyObjectByType<StageCount>();
            if (stageScript != null)
            {
                stageScript.OnItemCollected(); 
            }
            ItemManager.Instance.itemSpawnCount--;
            Destroy(gameObject);
        }
    }
}
