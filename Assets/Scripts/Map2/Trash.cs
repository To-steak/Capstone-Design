using UnityEngine;

public class Trash : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Trash_GameManager.Instance.AddScore(1);
            Destroy(gameObject);
        }
    }
}