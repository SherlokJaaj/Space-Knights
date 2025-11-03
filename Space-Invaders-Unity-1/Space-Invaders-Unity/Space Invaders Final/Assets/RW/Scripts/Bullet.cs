using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 200f;
    [SerializeField] private float lifeTime = 5f;

    void Awake()
    {
        // détruit automatiquement si hors écran / trop vieux
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        DestroySelf();
    }

    // utilisé par Torchka et autres
    public void DestroySelf()
    {
        // appelle l'explosion et détruit l'objet
        if (GameManager.Instance != null)
            GameManager.Instance.CreateExplosion((Vector2)transform.position);

        Destroy(gameObject);
    }
}
