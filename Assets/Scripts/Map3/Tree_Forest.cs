using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Image;
using Waste;

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
        minSeedRegenTerm = 5;
        maxSeedRegenTerm = 40;
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

    [SerializeField] private LayerMask _groundMask;
    private void SeedRegen()
    {

        if (curSeedRegenTerm <= 0)
        {
            Vector3 randomVecSpawnStartPoint = new Vector3(this.transform.position.x + UnityEngine.Random.Range(2, 8), 100f, this.transform.position.z + UnityEngine.Random.Range(2, 8));
            if (Physics.Raycast(randomVecSpawnStartPoint, Vector3.down, out RaycastHit hit, 300f, _groundMask))
            {
                Instantiate(_seed, hit.point + new Vector3(0, 1, 0), Quaternion.identity);
            }
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
