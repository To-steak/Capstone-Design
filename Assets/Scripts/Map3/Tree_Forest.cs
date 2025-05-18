using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Tree_Forest : MonoBehaviour
{
    private float HP = 100;
    public UnityEvent onLogged;

    public GameObject _seed;
    private float minSeedRegenTerm;
    private float maxSeedRegenTerm;
    private float curSeedRegenTerm;

    private GameObject _treePlace;
    private ForestManager _forestManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _forestManager = GameObject.Find("ForestManager").GetComponent<ForestManager>();
        if (_forestManager == null)
        {
            Debug.LogWarning("This scene has not contain ForestManager");
        }
    }
    void Start()
    {
        minSeedRegenTerm = 10;
        maxSeedRegenTerm = 30;
        curSeedRegenTerm = UnityEngine.Random.Range(minSeedRegenTerm, maxSeedRegenTerm);

        
    }
    private void OnEnable()
    {
        HP = 100;
    }
    // Update is called once per frame
    void Update()
    {
        if(HP <= 0)
        {
            onLogged.Invoke();
        }

        if (_forestManager.IsSeedRegenPossible())
        {
            SeedRegen();
        }
        
    }

    private void SeedRegen()
    {
        
        if (curSeedRegenTerm <= 0)
        {
            Vector3 randomVec = new Vector3(UnityEngine.Random.Range(2, 8), -this.transform.position.y + 2, UnityEngine.Random.Range(2, 8));
            Instantiate(_seed, this.transform.position + randomVec, Quaternion.identity);
            _forestManager.AddCurRegenSeeds(1);
            curSeedRegenTerm = UnityEngine.Random.Range(minSeedRegenTerm, maxSeedRegenTerm);
        }
        else
        {
            curSeedRegenTerm -= Time.deltaTime;
        }
    }

    public void AddHP(float amount)
    {
        HP += amount; 
        Debug.Log(this.ToString() + " / HP : " + HP);
    }

}
