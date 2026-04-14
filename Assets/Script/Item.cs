using UnityEngine;

public class Item : MonoBehaviour
{
    public int count = 1;
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerStats.Instance.currentCheese++;
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
            Debug.Log(ItemManager.Instance.itemSpawnCount);
            Destroy(gameObject);
        }
    }
}
