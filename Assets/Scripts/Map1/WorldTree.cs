using UnityEngine;

public class WorldTree : MonoBehaviour
{
    private int Health = 100;

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void HealthSet(int amount)
    {
        Health = amount;
    }

    public void HealthChange(int amount)
    {
        Health += amount;
    }


}
