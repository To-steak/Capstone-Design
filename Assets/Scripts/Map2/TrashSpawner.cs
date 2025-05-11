using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public GameObject trashPrefab;
    public int trashCount = 10;
    public Vector3 spawnArea = new Vector3(10, 0, 10);

    void Start()
    {
        for (int i = 0; i < trashCount; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
                0.5f,
                Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
            );

            Instantiate(trashPrefab, randomPosition, Quaternion.identity);
        }
    }
}