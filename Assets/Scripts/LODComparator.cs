using UnityEngine;

public class LODComparatorBasic : MonoBehaviour
{
    float timer = 0f;
    int frameCount = 0;

    void Update()
    {
        timer += Time.deltaTime;
        frameCount++;

        if (timer >= 5f)
        {
            float fps = frameCount / timer;
            int totalTris = EstimateTotalTris();

            Debug.Log($"[LOD TEST] FPS: {fps:F2}, Estimated Tris: {totalTris}");

            timer = 0f;
            frameCount = 0;
        }
    }

    int EstimateTotalTris()
    {
        int totalTris = 0;

        foreach (var mf in FindObjectsOfType<MeshFilter>())
        {
            Mesh mesh = mf.sharedMesh;
            if (mesh != null && mesh.isReadable)
            {
                totalTris += mesh.triangles.Length / 3;
            }
        }

        return totalTris;
    }

}
