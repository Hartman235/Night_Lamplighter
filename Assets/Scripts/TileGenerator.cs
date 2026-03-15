using UnityEngine;
using System.Collections.Generic;

public class TileGenerator : MonoBehaviour
{
    [Header("Tile Settings")]
    public GameObject[] tilePrefabs;
    public Transform player;
    public int startTiles = 6;
    public float tileLength = 100;
    
    [Header("Obstacle Settings")]
    public GameObject[] obstaclePrefabs;        // Препятствия
    [Range(0, 1)] public float obstacleChance = 0.5f;
    public int minObstacles = 1;
    public int maxObstacles = 3;
    
    [Header("Bonus Settings")]
    public GameObject[] bonusPrefabs;           // Бонусы (звезда, щит)
    [Range(0, 1)] public float bonusChance = 0.2f;
    
    [Header("Coin Settings")]
    public GameObject coinPrefab;
    [Range(0, 1)] public float coinChance = 0.8f;
    public int minCoins = 3;
    public int maxCoins = 6;
    
    [Header("Lane Settings")]
    public float laneWidth = 4f;                 // Ширина одной игровой полосы
    public int laneCount = 3;                     // Количество полос (всегда 3)
    
    [Header("Spacing Settings")]
    public float minDistanceBetweenObjects = 8f;  // Минимальное расстояние между объектами на дороге
    public float minDecorDistance = 20f;          // Минимальное расстояние между декорациями на одной стороне
    public float minLamppostCrossDistance = 15f;  // Минимальное расстояние между фонарями с разных сторон
    
    [Header("Decoration Settings")]
    public GameObject[] buildingPrefabs;          // Префабы зданий
    [Range(0, 1)] public float buildingChance = 0.8f;
    public int minBuildings = 2;
    public int maxBuildings = 4;
    
    public GameObject[] lamppostPrefabs;          // Префабы фонарных столбов
    [Range(0, 1)] public float lamppostChance = 0.5f;
    public int minLampposts = 1;
    public int maxLampposts = 2;
    
    [Header("Decoration Offsets")]
    public float buildingXOffset = 12f;           // Отступ зданий от центра дороги
    public float lamppostXOffset = 8f;            // Отступ столбов от центра дороги
    
    private List<GameObject> activeTiles = new List<GameObject>();
    private float spawnPos = 0;
    
    // Для отслеживания занятых позиций на текущем тайле (для препятствий/монет/бонусов)
    private List<Vector3> spawnedPositions = new List<Vector3>();
    
    // Списки для предотвращения наложения декораций
    private List<float> leftBuildingZ = new List<float>();
    private List<float> rightBuildingZ = new List<float>();
    private List<float> leftLamppostZ = new List<float>();
    private List<float> rightLamppostZ = new List<float>();
    
    void Start()
    {
        for (int i = 0; i < startTiles; ++i)
        {
            SpawnTile(Random.Range(0, tilePrefabs.Length));
        }
    }
    
    void Update()
    {
        if (player.position.z - 120 > spawnPos - (startTiles * tileLength))
        {
            SpawnTile(Random.Range(0, tilePrefabs.Length));
            DeleteTile();
        }
    }
    
    void SpawnTile(int tileIndex)
    {
        GameObject nextTile = Instantiate(
            tilePrefabs[tileIndex],
            transform.forward * spawnPos,
            transform.rotation
        );
        
        activeTiles.Add(nextTile);
        GenerateTileContent(nextTile, spawnPos);
        spawnPos += tileLength;
    }
    
    void GenerateTileContent(GameObject tile, float tileStartZ)
    {
        // Очищаем списки занятых позиций для этого тайла
        spawnedPositions.Clear();
        leftBuildingZ.Clear();
        rightBuildingZ.Clear();
        leftLamppostZ.Clear();
        rightLamppostZ.Clear();
        
        // --- Генерация объектов на дороге ---
        if (obstaclePrefabs.Length > 0 && Random.value < obstacleChance)
        {
            int obstacleCount = Random.Range(minObstacles, maxObstacles + 1);
            GenerateObstacles(tile, tileStartZ, obstacleCount);
        }
        
        if (coinPrefab != null && Random.value < coinChance)
        {
            int coinCount = Random.Range(minCoins, maxCoins + 1);
            GenerateCoins(tile, tileStartZ, coinCount);
        }
        
        if (bonusPrefabs.Length > 0 && Random.value < bonusChance)
        {
            GenerateBonus(tile, tileStartZ);
        }
        
        // --- Генерация зданий (могут стоять напротив друг друга) ---
        if (buildingPrefabs.Length > 0 && Random.value < buildingChance)
        {
            int buildingCount = Random.Range(minBuildings, maxBuildings + 1);
            // Левая сторона (проверяем только свою сторону)
            GenerateUniformDecorations(tile, tileStartZ, buildingCount, buildingPrefabs, 
                                       -buildingXOffset, leftBuildingZ, false, null);
            // Правая сторона (проверяем только свою сторону)
            GenerateUniformDecorations(tile, tileStartZ, buildingCount, buildingPrefabs, 
                                       buildingXOffset, rightBuildingZ, false, null);
        }
        
        // --- Генерация фонарей (не должны стоять напротив друг друга) ---
        if (lamppostPrefabs.Length > 0 && Random.value < lamppostChance)
        {
            int lamppostCount = Random.Range(minLampposts, maxLampposts + 1);
            // Левая сторона (проверяем свою сторону и правую сторону фонарей)
            GenerateUniformDecorations(tile, tileStartZ, lamppostCount, lamppostPrefabs, 
                                       -lamppostXOffset, leftLamppostZ, true, rightLamppostZ);
            // Правая сторона (проверяем свою сторону и левую сторону фонарей)
            GenerateUniformDecorations(tile, tileStartZ, lamppostCount, lamppostPrefabs, 
                                       lamppostXOffset, rightLamppostZ, true, leftLamppostZ);
        }
    }
    
    bool IsPositionFree(float x, float z, float minDistance)
    {
        foreach (Vector3 pos in spawnedPositions)
        {
            float distance = Vector2.Distance(
                new Vector2(x, z),
                new Vector2(pos.x, pos.z)
            );
            if (distance < minDistance)
                return false;
        }
        return true;
    }
    
    void GenerateObstacles(GameObject tile, float tileStartZ, int count)
    {
        List<int> availableLanes = new List<int> { 0, 1, 2 };
        float segmentLength = tileLength / (count + 1);
        
        for (int i = 0; i < count; i++)
        {
            int attempts = 0;
            int maxAttempts = 15;
            bool spawned = false;
            
            float segmentCenter = tileStartZ + (i + 1) * segmentLength;
            float zMin = segmentCenter - segmentLength * 0.25f;
            float zMax = segmentCenter + segmentLength * 0.25f;
            
            Shuffle(availableLanes);
            
            while (!spawned && attempts < maxAttempts)
            {
                attempts++;
                foreach (int lane in availableLanes)
                {
                    float xPos = (lane - 1) * laneWidth;
                    float zPos = Random.Range(zMin, zMax);
                    
                    if (zPos < tileStartZ + 15f || zPos > tileStartZ + tileLength - 15f)
                        continue;
                    
                    if (IsPositionFree(xPos, zPos, minDistanceBetweenObjects))
                    {
                        int obstacleIndex = Random.Range(0, obstaclePrefabs.Length);
                        GameObject obstacle = Instantiate(
                            obstaclePrefabs[obstacleIndex],
                            new Vector3(xPos, 1.5f, zPos),
                            Quaternion.identity
                        );
                        obstacle.transform.SetParent(tile.transform);
                        obstacle.tag = "obstacle";
                        EnsureCollider(obstacle);
                        
                        spawnedPositions.Add(new Vector3(xPos, 0, zPos));
                        spawned = true;
                        break;
                    }
                }
            }
            
            if (!spawned)
            {
                TrySpawnObstacleAnywhere(tile, tileStartZ);
            }
        }
    }
    
    void TrySpawnObstacleAnywhere(GameObject tile, float tileStartZ)
    {
        int attempts = 0;
        int maxAttempts = 20;
        while (attempts < maxAttempts)
        {
            attempts++;
            int lane = Random.Range(0, laneCount);
            float xPos = (lane - 1) * laneWidth;
            float zPos = tileStartZ + Random.Range(20f, 80f);
            
            if (IsPositionFree(xPos, zPos, minDistanceBetweenObjects))
            {
                int obstacleIndex = Random.Range(0, obstaclePrefabs.Length);
                GameObject obstacle = Instantiate(
                    obstaclePrefabs[obstacleIndex],
                    new Vector3(xPos, 1.5f, zPos),
                    Quaternion.identity
                );
                obstacle.transform.SetParent(tile.transform);
                obstacle.tag = "obstacle";
                EnsureCollider(obstacle);
                
                spawnedPositions.Add(new Vector3(xPos, 0, zPos));
                return;
            }
        }
    }
    
    void GenerateCoins(GameObject tile, float tileStartZ, int count)
    {
        bool createCoinLine = Random.value < 0.5f;
        if (createCoinLine && count >= 3)
        {
            int lane = Random.Range(0, laneCount);
            float xPos = (lane - 1) * laneWidth;
            int lineLength = Random.Range(3, 5);
            float startZ = tileStartZ + Random.Range(30f, 50f);
            
            for (int i = 0; i < lineLength; i++)
            {
                float zPos = startZ + i * minDistanceBetweenObjects * 0.8f;
                if (zPos < tileStartZ + tileLength - 20f)
                {
                    if (IsPositionFree(xPos, zPos, minDistanceBetweenObjects * 0.5f))
                    {
                        GameObject coin = Instantiate(
                            coinPrefab,
                            new Vector3(xPos, 2f, zPos),
                            Quaternion.identity
                        );
                        coin.transform.SetParent(tile.transform);
                        coin.tag = "coin";
                        AddRotationIfMissing(coin);
                        spawnedPositions.Add(new Vector3(xPos, 0, zPos));
                    }
                }
            }
            int remainingCoins = count - lineLength;
            for (int i = 0; i < remainingCoins; i++)
            {
                SpawnRandomCoin(tile, tileStartZ);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                SpawnRandomCoin(tile, tileStartZ);
            }
        }
    }
    
    void SpawnRandomCoin(GameObject tile, float tileStartZ)
    {
        int attempts = 0;
        int maxAttempts = 15;
        while (attempts < maxAttempts)
        {
            attempts++;
            int lane = Random.Range(0, laneCount);
            float xPos = (lane - 1) * laneWidth;
            float zPos = tileStartZ + Random.Range(15f, 85f);
            
            if (IsPositionFree(xPos, zPos, minDistanceBetweenObjects * 0.5f))
            {
                GameObject coin = Instantiate(
                    coinPrefab,
                    new Vector3(xPos, 2f, zPos),
                    Quaternion.identity
                );
                coin.transform.SetParent(tile.transform);
                coin.tag = "coin";
                AddRotationIfMissing(coin);
                spawnedPositions.Add(new Vector3(xPos, 0, zPos));
                return;
            }
        }
    }
    
    void GenerateBonus(GameObject tile, float tileStartZ)
    {
        int attempts = 0;
        int maxAttempts = 15;
        while (attempts < maxAttempts)
        {
            attempts++;
            int lane = Random.Range(0, laneCount);
            float xPos = (lane - 1) * laneWidth;
            float zPos = tileStartZ + Random.Range(30f, 70f);
            
            if (IsPositionFree(xPos, zPos, minDistanceBetweenObjects))
            {
                int bonusIndex = Random.Range(0, bonusPrefabs.Length);
                GameObject bonus = Instantiate(
                    bonusPrefabs[bonusIndex],
                    new Vector3(xPos, 2f, zPos),
                    Quaternion.identity
                );
                bonus.transform.SetParent(tile.transform);
                AddRotationIfMissing(bonus);
                spawnedPositions.Add(new Vector3(xPos, 0, zPos));
                return;
            }
        }
    }
    
    /// <summary>
    /// Равномерно распределяет объекты по длине тайла
    /// </summary>
    /// <param name="checkOppositeSide">Нужно ли проверять расстояние с объектами на противоположной стороне</param>
    /// <param name="oppositeSideList">Список позиций на противоположной стороне (если checkOppositeSide = true)</param>
    void GenerateUniformDecorations(GameObject tile, float tileStartZ, int count, GameObject[] prefabs, 
                                    float xOffset, List<float> usedZList, bool checkOppositeSide, List<float> oppositeSideList)
    {
        if (count <= 0) return;

        float segmentLength = tileLength / (count + 1);
        float segmentRange = segmentLength * 0.25f;

        for (int i = 0; i < count; i++)
        {
            float segmentCenter = tileStartZ + (i + 1) * segmentLength;
            float zMin = segmentCenter - segmentRange;
            float zMax = segmentCenter + segmentRange;

            int attempts = 0;
            int maxAttempts = 20;
            bool spawned = false;

            while (!spawned && attempts < maxAttempts)
            {
                attempts++;
                float zPos = Random.Range(zMin, zMax);

                // Отступ от краёв тайла
                if (zPos < tileStartZ + 15f || zPos > tileStartZ + tileLength - 15f)
                    continue;

                // Проверка расстояния с объектами на этой же стороне
                bool tooClose = false;
                foreach (float existingZ in usedZList)
                {
                    if (Mathf.Abs(existingZ - zPos) < minDecorDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (tooClose) continue;

                // Если нужно проверить противоположную сторону
                if (checkOppositeSide && oppositeSideList != null)
                {
                    foreach (float existingZ in oppositeSideList)
                    {
                        if (Mathf.Abs(existingZ - zPos) < minLamppostCrossDistance)
                        {
                            tooClose = true;
                            break;
                        }
                    }
                    if (tooClose) continue;
                }

                float yPos = 0f;
                int index = Random.Range(0, prefabs.Length);
                GameObject obj = Instantiate(prefabs[index], new Vector3(xOffset, yPos, zPos), Quaternion.identity);
                obj.transform.SetParent(tile.transform);
                usedZList.Add(zPos);
                spawned = true;
            }
        }
    }
    
    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    
    void EnsureCollider(GameObject obj)
    {
        if (obj.GetComponent<Collider>() == null)
        {
            BoxCollider collider = obj.AddComponent<BoxCollider>();
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                collider.size = meshFilter.sharedMesh.bounds.size;
                collider.center = meshFilter.sharedMesh.bounds.center;
            }
        }
    }
    
    void AddRotationIfMissing(GameObject obj)
    {
        if (obj.GetComponent<BonusRotate>() == null)
        {
            obj.AddComponent<BonusRotate>();
        }
    }
    
    void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }
}