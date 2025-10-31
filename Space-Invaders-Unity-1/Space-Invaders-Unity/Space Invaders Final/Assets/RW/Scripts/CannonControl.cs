using UnityEngine;
using System.Collections;

public class CannonControl : MonoBehaviour
{
    public float speed = 500f;
    public Transform muzzle;
    public AudioClip shooting;
    public float coolDownTime = 0.5f;
    public Bullet bulletPrefab;

    private float shootTimer;

    public float respawnTime = 2f;
    public SpriteRenderer sprite;
    public Collider2D cannonCollider;

    private Vector2 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D))
            transform.Translate(speed * Time.deltaTime, 0, 0);
        else if (Input.GetKey(KeyCode.A))
            transform.Translate(-speed * Time.deltaTime, 0, 0);

        shootTimer += Time.deltaTime;
        if (shootTimer > coolDownTime && Input.GetKey(KeyCode.Space))
        {
            shootTimer = 0f;
            if (bulletPrefab != null && muzzle != null)
            {
                // spawn un peu devant le muzzle pour éviter overlap
                Vector3 spawnPos = muzzle.position + Vector3.up * 0.1f;
                Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            }
            if (GameManager.Instance != null && shooting != null)
                GameManager.Instance.PlaySfx(shooting);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (GameManager.Instance != null) GameManager.Instance.UpdateLives();
        StopAllCoroutines();
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        enabled = false;
        cannonCollider.enabled = false;
        ChangeSpriteAlpha(0.0f);

        yield return new WaitForSeconds(0.25f * respawnTime);

        transform.position = startPos;
        enabled = true;
        ChangeSpriteAlpha(0.25f);

        yield return new WaitForSeconds(0.75f * respawnTime);

        ChangeSpriteAlpha(1.0f);
        cannonCollider.enabled = true;
    }

    private void ChangeSpriteAlpha(float value)
    {
        if (sprite == null) return;
        var color = sprite.color;
        color.a = value;
        sprite.color = color;
    }
}
