using System.Collections.Generic;
using UnityEngine;

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

    public static InvaderSwarm Instance;

    [Header("Spawning")]
    [SerializeField] private InvaderType[] invaderTypes;
    [SerializeField] private int columnCount = 11;
    [SerializeField] private int ySpacing;
    [SerializeField] private int xSpacing;
    [SerializeField] private Transform spawnStartPoint;

    private float minX;

    [Header("Movement")]
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float speedIncreasePerWave = 0.05f;
    [SerializeField] private float maxSpeedMultiplier = 2.5f;

    private Transform[,] invaders;
    private int rowCount;
    private bool isMovingRight = true;
    private float maxX;
    private float currentX;
    private float xIncrement;

    [SerializeField] private BulletSpawner bulletSpawnerPrefab;
    [SerializeField] private MusicControl musicControl;
    [SerializeField] private Transform cannonPosition;

    private int killCount;
    private int tempKillCount;
    private Dictionary<string, int> pointsMap;

    private int waveNumber = 1;
    private float minY;
    private float currentY;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        InitializeSwarm();
    }

    private void Update()
    {
        float tempo = (musicControl != null) ? musicControl.Tempo : 1f;

        // vitesse progressive par vague (logarithmique pour limiter la croissance)
        float waveSpeedBonus = 1f + Mathf.Log10(1f + (waveNumber - 1) * 10f * speedIncreasePerWave);
        waveSpeedBonus = Mathf.Min(waveSpeedBonus, maxSpeedMultiplier);

        xIncrement = baseSpeed * tempo * waveSpeedBonus * Time.deltaTime;

        if (isMovingRight)
        {
            currentX += xIncrement;
            if (currentX < maxX)
                MoveInvaders(xIncrement, 0);
            else
                ChangeDirection();
        }
        else
        {
            currentX -= xIncrement;
            if (currentX > minX)
                MoveInvaders(-xIncrement, 0);
            else
                ChangeDirection();
        }
    }

    private void MoveInvaders(float x, float y)
    {
        if (invaders == null) return;

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                var t = invaders[i, j];
                if (t == null) continue;
                t.Translate(x, y, 0);
            }
        }
    }

    private void ChangeDirection()
    {
        isMovingRight = !isMovingRight;
        MoveInvaders(0, -ySpacing);

        currentY -= ySpacing;
        if (currentY < minY)
        {
            GameManager.Instance.TriggerGameOver();
        }
    }

    public void IncreaseDeathCount()
    {
        killCount++;
        if (killCount >= (invaders != null ? invaders.Length : 0))
        {
            waveNumber++;
            StartNextWave();
            return;
        }

        tempKillCount++;
        int threshold = (invaders != null && musicControl != null && musicControl.pitchChangeSteps > 0)
                        ? Mathf.Max(1, invaders.Length / musicControl.pitchChangeSteps)
                        : int.MaxValue;

        if (tempKillCount >= threshold)
        {
            musicControl?.IncreasePitch();
            tempKillCount = 0;
        }
    }

    public int GetPoints(string alienName)
    {
        if (pointsMap == null) return 0;
        if (pointsMap.ContainsKey(alienName)) return pointsMap[alienName];
        return 0;
    }

    public Transform GetInvader(int row, int column)
    {
        if (invaders == null) return null;
        if (row < 0 || column < 0 || row >= invaders.GetLength(0) || column >= invaders.GetLength(1)) return null;
        return invaders[row, column];
    }

    // --------------------------- Helper Methods ---------------------------

    private void InitializeSwarm()
    {
        if (spawnStartPoint == null)
        {
            Debug.LogError("InvaderSwarm: spawnStartPoint not assigned.");
            return;
        }

        currentY = spawnStartPoint.position.y;
        minY = cannonPosition != null ? cannonPosition.position.y : float.MinValue;
        minX = spawnStartPoint.position.x;

        GameObject swarm = new GameObject { name = "Swarm" };
        Vector2 currentPos = spawnStartPoint.position;

        rowCount = 0;
        foreach (var invaderType in invaderTypes) rowCount += invaderType.rowCount;

        maxX = minX + 2f * xSpacing * columnCount;
        currentX = minX;

        invaders = new Transform[rowCount, columnCount];
        pointsMap = new Dictionary<string, int>();

        int rowIndex = 0;
        foreach (var invaderType in invaderTypes)
        {
            string invaderName = invaderType.name.Trim();
            pointsMap[invaderName] = invaderType.points;

            for (int i = 0; i < invaderType.rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    var invader = new GameObject { name = invaderName };
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
        }

        for (int i = 0; i < columnCount; i++)
        {
            var bulletSpawner = Instantiate(bulletSpawnerPrefab);
            bulletSpawner.transform.SetParent(swarm.transform);
            bulletSpawner.column = i;
            bulletSpawner.currentRow = rowCount - 1;
            bulletSpawner.Setup();
        }

        killCount = 0;
        tempKillCount = 0;
    }

private void StartNextWave()
{
    // nettoyage de l'ancien swarm
    ClearOldSwarm();

    // toutes les 3 vagues, ajouter une ligne d'un type spécifique
    if (waveNumber % 3 == 0)
    {
        int typeIndex = (waveNumber / 3 - 1) % invaderTypes.Length;
        invaderTypes[typeIndex].rowCount += 1;
    }

    InitializeSwarm();
}



    private void ClearOldSwarm()
    {
        // détruit tous les enfants de ce GameObject (swarm et bullets)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
