using UnityEngine;

public class Item : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
