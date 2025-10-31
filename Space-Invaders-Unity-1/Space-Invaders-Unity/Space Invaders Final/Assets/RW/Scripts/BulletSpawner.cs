using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public int currentRow;
    public int column;

    public AudioClip shooting;
    public GameObject bulletPrefab;
    public Transform spawnPoint;

    public float minTime;
    public float maxTime;

    private float timer;
    private float currentTime;
    private Transform followTarget;

    public void Setup()
    {
        currentTime = Random.Range(minTime, maxTime);
        followTarget = InvaderSwarm.Instance != null ? InvaderSwarm.Instance.GetInvader(currentRow, column) : null;
    }

    void Update()
    {
        if (followTarget != null)
            transform.position = followTarget.position;

        timer += Time.deltaTime;
        if (timer < currentTime) return;

        // spawn légèrement en dehors pour éviter overlap instantané avec l'invader
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position + Vector3.down * 0.1f : transform.position;
        if (bulletPrefab != null)
            Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        if (GameManager.Instance != null && shooting != null)
            GameManager.Instance.PlaySfx(shooting);

        timer = 0f;
        currentTime = Random.Range(minTime, maxTime);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // on ne gère que les balles du joueur
        var playerBullet = other.collider.GetComponent<Bullet>();
        if (playerBullet == null) return;
        if (followTarget == null) return;

        // update score via points map (invader name)
        var invaderName = followTarget.gameObject.name;
        if (InvaderSwarm.Instance != null)
            GameManager.Instance.UpdateScore(InvaderSwarm.Instance.GetPoints(invaderName));

        // notify swarm
        InvaderSwarm.Instance?.IncreaseDeathCount();

        // masquer sprite de l'invader touché (garde la transform pour le spawner)
        var sr = followTarget.GetComponentInChildren<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        currentRow = currentRow - 1;

        if (currentRow < 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Setup();
        }
    }
}
