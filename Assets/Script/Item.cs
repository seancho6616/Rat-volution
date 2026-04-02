using UnityEngine;

public class Item : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Gauge gaugeScript = FindAnyObjectByType<Gauge>();
            if(gaugeScript != null)
            {
                gaugeScript.AddScore(1);
            }

            gameObject.SetActive(false);
        }
    }
}
