using UnityEngine;

public class WorldTree : MonoBehaviour
{
    private float Health = 100;

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void SetHealth(float amount)
    {
        Health = amount;
        Debug.Log("World Tree Health set : " + Health);
    }

    public void HealthChange(int amount)
    {
        Health += amount;
        Debug.Log("World Tree Health : " +  Health);
    }

    public float GetHealth()
    {
        return Health;
    }
}
