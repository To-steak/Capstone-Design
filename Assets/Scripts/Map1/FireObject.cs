using UnityEngine;
using UnityEngine.Events;

public class FireObject : ObjectInteraction
{
    private WorldTreeManager _worldTreeManager;
    public ParticleSystem fire;

    private void Awake()
    {
        _worldTreeManager = GameObject.Find("WorldTreeManager").GetComponent<WorldTreeManager>();
        if (_worldTreeManager == null)
        {
            Debug.LogWarning("This scene has not contain WorldTreeManager");
        }
    }

    public void WhenPlayerInteractToFire()
    {
        fire.Stop();
        _worldTreeManager.FireExtinguishing();
    }
}
