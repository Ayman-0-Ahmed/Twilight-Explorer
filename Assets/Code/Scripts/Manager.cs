using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class Manager : MonoBehaviour {
    public float spawnRadius;
    public TerrainGenerator terrainGenerator;
    public ThirdPersonController playerController;
    public GameObject objectToSpawn;
    public Transform EnemyParent;
    public float spawnInterval = 5f;
    public List<EnemyAI> enemies = new();
    float time = 0f;
    int spawn;
    void Update() {
        if (spawn > 5) return;
        time += Time.deltaTime;
        if (time >= spawnInterval) {
            time = 0f;
            SpawnEnemies();                
        }
    }
    void SpawnEnemies()
    {
        List<Vector2> points = PoissonDiskSampler.GeneratePoints(spawnRadius, terrainGenerator.width, terrainGenerator.depth, terrainGenerator.spawnAttempts);
        int halfWidth = terrainGenerator.width / 2;
        int halfDepth = terrainGenerator.depth / 2;
        Vector2 offset = terrainGenerator.offset;
        float scale = terrainGenerator.scale;
        float heightMultiplier = terrainGenerator.heightMultiplier;
        EnemyParent.localScale = Vector3.one;
        EnemyParent.localPosition = Vector3.zero;
        objectToSpawn.SetActive(true);
        foreach (Vector2 point in points)
        {
            float y = Mathf.PerlinNoise((point.x + offset.x) / scale, (point.y + offset.y) / scale) * heightMultiplier;
            Vector3 position = new(point.x - halfWidth, y, point.y - halfDepth);
            EnemyAI e = Instantiate(objectToSpawn, position, Quaternion.identity, EnemyParent).GetComponent<EnemyAI>();
            if (playerController.lightMode) e.StartFleeing();
            enemies.Add(e);
        }
        objectToSpawn.SetActive(false);
        EnemyParent.localScale = terrainGenerator.transform.localScale;
        EnemyParent.localPosition = terrainGenerator.transform.localPosition;
        spawn++;
    }
}
