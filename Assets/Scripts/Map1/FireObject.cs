using UnityEngine;
using UnityEngine.Events;

public class FireObject : ObjectInteraction
{
    private WorldTreeManager _worldTreeManager;
    public ParticleSystem fire;

    public UnityEvent fireOverflow;
    private bool isLit = false;
    private float fireTimer = 0f;
    private float fireTimeOut = 10f;


    private void Awake()
    {
        _worldTreeManager = GameObject.Find("WorldTreeManager").GetComponent<WorldTreeManager>();
        if (_worldTreeManager == null)
        {
            Debug.LogWarning("This scene has not contain WorldTreeManager");
        }

        
    }

    private void Start()
    {
        fireTimeOut = _worldTreeManager.GetTermOfFireOverflow();
        fire.Stop();
        fire.GetComponent<AudioSource>().Stop();
    }
    private void Update()
    {
        
        if (fireTimer >= fireTimeOut && isLit)
        {
            fireTimer = 0f;
            fireOverflow.Invoke();
            Debug.Log(this.name + " : fire overflow invoke");
        }
        else if (fireTimer < fireTimeOut && isLit)
        {
            fireTimer += Time.deltaTime;
        }

    }

    public void SetLit(bool state)
    {
        if (isLit != state)
        {
            if (state)
            {
                fire.Play();
                fire.GetComponent<AudioSource>().Play();
            }
            else
            {
                fire.Stop();
                fire.GetComponent<AudioSource>().Stop();
                fireTimer = 0f;
            }
        }

        isLit = state;
    }

    public void WhenPlayerTurnOffFire()
    {
        if (isLit)
        {
            fire.Stop();
            fire.GetComponent<AudioSource>().Stop();
            fireTimer = 0f;
            _worldTreeManager.FireExtinguishing();
            isLit = false;
        }
    }

    public bool GetLit() { return isLit; }
}
