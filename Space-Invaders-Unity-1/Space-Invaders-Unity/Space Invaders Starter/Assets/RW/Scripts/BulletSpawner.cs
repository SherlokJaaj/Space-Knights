
using UnityEngine;

namespace RayWenderlich.SpaceInvadersUnity
{
    public class BulletSpawner : MonoBehaviour
    {
        internal int currentRow;
        internal int column;

        [SerializeField]
        private AudioClip shooting;

        [SerializeField]
        private GameObject bulletPrefab;

        [SerializeField]
        private Transform spawnPoint;

        [SerializeField]
        private float minTime;

        [SerializeField]
        private float maxTime;

        private float timer;
        private float currentTime;
        private Transform followTarget;

        
        internal void Setup()
        {
            currentTime = Random.Range(minTime, maxTime);
            followTarget = InvaderSwarm.Instance.GetInvader(currentRow, column);
        }

        private void Update()
        {
            transform.position = followTarget.position;

            timer += Time.deltaTime;
            if (timer < currentTime)
            {
                return;
            }

            Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
            GameManager.Instance.PlaySfx(shooting);
            timer = 0f;
            currentTime = Random.Range(minTime, maxTime);
        }

    }
}