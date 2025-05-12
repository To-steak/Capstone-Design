using UnityEngine;

public class DeactivateWhenStart : MonoBehaviour
{
    private void Awake()
    {
        this.gameObject.SetActive(false);
    }
}
