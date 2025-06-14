using System.Collections.Generic;
using UnityEngine;

public static class PoissonDiskSampler {
    public static List<Vector2> GeneratePoints(float radius, float width, float height, int numSamplesBeforeRejection = 30) {
        float cellSize = radius / Mathf.Sqrt(2);
        int[,] grid = new int[Mathf.CeilToInt(width / cellSize), Mathf.CeilToInt(height / cellSize)];
        List<Vector2> points = new();
        List<Vector2> spawnPoints = new() { new(width / 2, height / 2)};

        while (spawnPoints.Count > 0) {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCentre = spawnPoints[spawnIndex];
            bool accepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++) {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                float mag = Random.Range(radius, 2 * radius);
                Vector2 candidate = spawnCentre + dir * mag;

                if (IsValid(candidate, width, height, cellSize, radius, points, grid)) {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    accepted = true;
                    break;
                }
            }

            if (!accepted) spawnPoints.RemoveAt(spawnIndex);
        }

        return points;
    }

    private static bool IsValid(Vector2 candidate, float width, float height, float cellSize, float radius, List<Vector2> points, int[,] grid) {
        if (candidate.x < 0 || candidate.x >= width || candidate.y < 0 || candidate.y >= height)
            return false;

        int cellX = (int)(candidate.x / cellSize);
        int cellY = (int)(candidate.y / cellSize);

        int searchStartX = Mathf.Max(0, cellX - 2);
        int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
        int searchStartY = Mathf.Max(0, cellY - 2);
        int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

        for (int x = searchStartX; x <= searchEndX; x++) {
            for (int y = searchStartY; y <= searchEndY; y++) {
                int index = grid[x, y] - 1;
                if (index != -1) {
                    float dist = Vector2.Distance(candidate, points[index]);
                    if (dist < radius) return false;
                }
            }
        }

        return true;
    }
}
