using UnityEngine;
using UnityEngine.Events;

public class Trash_HP : MonoBehaviour
{
    private float HP = 100;
    public UnityEvent onLogged;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    private void OnEnable()
    {
        HP = 100;
    }
    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            onLogged.Invoke();
        }
    }

    public void AddHP(float amount)
    {
        HP += amount;
        Debug.Log(this.ToString() + " / HP : " + HP);
    }

}
