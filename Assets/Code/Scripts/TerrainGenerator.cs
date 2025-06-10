using UnityEngine;
using System.Collections.Generic;
using Unity.AI.Navigation;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshCollider))]
public class TerrainGenerator : MonoBehaviour {
    public NavMeshSurface navMeshSurface;
    public int width = 100;
    public int depth = 100;
    public float scale = 20f;

    public float heightMultiplier = 5f;
    public Vector2 offset;
    public Transform objectParent;
    public List<GameObject> objectsToSpawn;
    public float spawnRadius = 5f;
    public int spawnAttempts = 30;

    Mesh mesh;
    MeshCollider meshCollider;
    MeshFilter meshFilter;
    Vector3[] vertices;

    void Start()
    {
        if (navMeshSurface.navMeshData == null) navMeshSurface.BuildNavMesh();
    }

    public void GenerateTerrain() {
        mesh = new Mesh() { name = "TerrainMesh" };
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();

        vertices = new Vector3[(width + 1) * (depth + 1)];
        int[] triangles = new int[width * depth * 6];
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int z = 0, i = 0; z <= depth; z++) {
            for (int x = 0; x <= width; x++, i++) {
                float y = Mathf.PerlinNoise((x + offset.x) / scale, (z + offset.y) / scale) * heightMultiplier;
                vertices[i] = new Vector3(x-(width/2), y, z-(depth/2));
                uvs[i] = new Vector2((float)x / width, (float)z / depth);
            }
        }

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < depth; z++) {
            for (int x = 0; x < width; x++) {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    public void SpawnObjects()
    {
        List<Vector2> points = PoissonDiskSampler.GeneratePoints(spawnRadius, width, depth, spawnAttempts);
        int halfWidth = width / 2;
        int halfDepth = depth / 2;
        objectParent.localScale = Vector3.one;
        objectParent.localPosition = Vector3.zero;
        foreach (Vector2 point in points)
        {
            float y = Mathf.PerlinNoise((point.x + offset.x) / scale, (point.y + offset.y) / scale) * heightMultiplier;
            Vector3 position = new(point.x - halfWidth, y, point.y - halfDepth);
            Object objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Count)];
            Instantiate(objectToSpawn, position, Quaternion.identity, objectParent);
        }
        objectParent.localScale = transform.localScale;
        objectParent.localPosition = transform.localPosition;
    }
}
