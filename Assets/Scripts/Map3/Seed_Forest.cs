using UnityEngine;

public class Seed_Forest : MonoBehaviour
{
    private ForestManager _forestManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _forestManager = GameObject.Find("ForestManager").GetComponent<ForestManager>();
        if (_forestManager == null)
        {
            Debug.LogWarning("This scene has not contain ForestManager");
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _forestManager.addHaveSeedCount(1);
            Debug.Log("Player get Seed");
            Destroy(this.gameObject);
        }
    }
}
