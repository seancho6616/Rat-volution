using UnityEngine;

public class Bomb : MonoBehaviour
{
    public static Bomb Instance;
    public ParticleSystem particle;
    private void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }
    Instance = this;
}

    public void ParticlePlay(Vector3 vector3)
    {
        Debug.Log(vector3);
        transform.position = vector3;
        particle.Clear();
        particle.Play();
    }
}
