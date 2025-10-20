
using UnityEngine;

namespace RayWenderlich.SpaceInvadersUnity
{
    public class InvaderSwarm : MonoBehaviour
    {
        [System.Serializable]
        private struct InvaderType
        {
            public string name;
            public Sprite[] sprites;
            public int points;
            public int rowCount;
        }
        internal static InvaderSwarm Instance;

        [Header("Spawning")]
        [SerializeField]
        private InvaderType[] invaderTypes;

        [SerializeField]
        private int columnCount = 11;

        [SerializeField]
        private int ySpacing;

        [SerializeField]
        private int xSpacing;

        [SerializeField]
        private Transform spawnStartPoint;

        private float minX;

        [Space]
        [Header("Movement")]
        [SerializeField]
        private float speedFactor = 10f;

        private Transform[,] invaders;
        private int rowCount;
        private bool isMovingRight = true;
        private float maxX;
        private float currentX;
        private float xIncrement;

        public GameObject bulletSpawnerPrefab;
        internal Transform GetInvader(int row, int column)
        {
            if (row < 0 || column < 0
                || row >= invaders.GetLength(0) || column >= invaders.GetLength(1))
            {
                return null;
            }

            return invaders[row, column];
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            minX = spawnStartPoint.position.x;

            GameObject swarm = new GameObject { name = "Swarm" };
            Vector2 currentPos = spawnStartPoint.position;

            foreach (var invaderType in invaderTypes)
            {
                rowCount += invaderType.rowCount;
            }
            maxX = minX + 2f * xSpacing * columnCount;
            currentX = minX;
            invaders = new Transform[rowCount, columnCount];

            int rowIndex = 0;
            foreach (var invaderType in invaderTypes)
            {
                var invaderName = invaderType.name.Trim();
                for (int i = 0, len = invaderType.rowCount; i < len; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        var invader = new GameObject() { name = invaderName };
                        invader.AddComponent<SimpleAnimator>().sprites = invaderType.sprites;
                        invader.transform.position = currentPos;
                        invader.transform.SetParent(swarm.transform);

                        invaders[rowIndex, j] = invader.transform;


                        currentPos.x += xSpacing;
                    }

                    currentPos.x = minX;
                    currentPos.y -= ySpacing;

                    rowIndex++;
                }
                for (int i = 0; i < columnCount; i++)
                {
                    var bulletSpawner = Instantiate(bulletSpawnerPrefab);
                    //bulletSpawner.transform.SetParent(swarm.transform);
                    //bulletSpawner.column = i;
                    //bulletSpawner.currentRow = rowCount - 1;
                    //bulletSpawner.Setup();
                }
            }
        }

        private void Update()
        {
            xIncrement = speedFactor * Time.deltaTime;
            if (isMovingRight)
            {
                currentX += xIncrement;
                if (currentX < maxX)
                {
                    MoveInvaders(xIncrement, 0);
                }
                else
                {
                    ChangeDirection();
                }
            }
            else
            {
                currentX -= xIncrement;
                if (currentX > minX)
                {
                    MoveInvaders(-xIncrement, 0);
                }
                else
                {
                    ChangeDirection();
                }
            }
        }

        private void MoveInvaders(float x, float y)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    invaders[i, j].Translate(x, y, 0);
                }
            }
        }

        private void ChangeDirection()
        {
            isMovingRight = !isMovingRight;
            MoveInvaders(0, -ySpacing);

        }

    }
}