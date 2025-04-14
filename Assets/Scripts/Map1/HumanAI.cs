using UnityEngine;

public class HumanAI : MonoBehaviour
{
    private int Health = 100;

    private WorldTreeManager _worldTreeManager;

    private void Awake()
    {
        _worldTreeManager = GameObject.Find("WorldTreeManager").GetComponent<WorldTreeManager>();
        if (_worldTreeManager == null)
        {
            Debug.LogWarning("This scene has not contain WorldTreeManager");
        }
    }

    void Update()
    {
        if (Health <= 0)
        {
            Destroy(this);

        }
    }

    public void WhenPlayerInteractToFire()
    {
        _worldTreeManager.HumanHunting();
    }

    public void HealthChange(int amount)
    {
        Health += amount;
    }

}
