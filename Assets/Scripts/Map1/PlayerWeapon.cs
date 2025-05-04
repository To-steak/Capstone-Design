using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

public class PlayerWeapon : MonoBehaviour, WorldTreeWeaponSystem.IWeaponActions
{
    public Vector3 offset = new Vector3(0, 0, 0);
    public float weaponDistance = 10f;
    public float weaponDamage = 40;

    public WorldTreeWeaponSystem WorldTreeWeaponSystem { get; private set; }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        WorldTreeWeaponSystem = new WorldTreeWeaponSystem();
        WorldTreeWeaponSystem.Enable();

        WorldTreeWeaponSystem.Weapon.Enable();
        WorldTreeWeaponSystem.Weapon.SetCallbacks(this);
    }

    private void OnDisable()
    {
        WorldTreeWeaponSystem.Weapon.Disable();
        WorldTreeWeaponSystem.Weapon.RemoveCallbacks(this);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        
        if (context.started)
        {
            Debug.Log("Attack");
            Scan();
        }
        
    }

    [SerializeField]
    LayerMask layer;

    void Scan()
    {
        Vector3 origin = transform.position + offset;
        Vector3 dir = transform.forward;
        Ray ray = new Ray(origin, dir);
        RaycastHit rayHit;
        
        

        if(Physics.Raycast(ray, out rayHit, weaponDistance, layer))
        {
            HumanAI target = rayHit.collider.GetComponent<HumanAI>();
            if ( target != null)
            {
                Debug.Log("ray Hit HumanAI", rayHit.transform);
                target.TakeDamage(weaponDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + offset;
        Vector3 dir = transform.forward;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + dir * weaponDistance);
    }
}
