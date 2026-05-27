using UnityEngine;

public class Bomb : MonoBehaviour
{
    public static Bomb Instance;
    public ParticleSystem particle;

    [Header("Audio Sources")]
    public AudioSource bombSound;

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

        // --- 파티클이 재생될 때 사운드도 함께 재생 ---
        if (bombSound != null && bombSound.clip != null)
        {
            // 여러 벽이 동시에 터져도 소리가 끊기지 않도록 PlayOneShot 사용
            bombSound.PlayOneShot(bombSound.clip, bombSound.volume);
        }
    }
}
