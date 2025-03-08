using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("Player Attribute")]
    public float MoveSpeed;
    public float JumpAmount;
    public float MouseSensitivity;

    [Header("Camera Attribute")]
    public GameObject CinemachineCameraTarget;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private Vector3 v_moveDirection;
    private Vector2 v_lookDirection;
    private Rigidbody rb_rigidbody;
    private CharacterController cc_characterController;
    private bool b_isGrounded;

    void Awake()
    {
        rb_rigidbody = GetComponent<Rigidbody>();
        cc_characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    void Update()
    {
        Move(v_moveDirection, MoveSpeed);
        Look(v_lookDirection);
    }

    void LateUpdate()
    {
        CameraRotation();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        if (value != null)
        {
            v_moveDirection = new Vector3(value.x, 0.0f, value.y);
            Debug.Log($"On Move: {v_moveDirection}");
        }
    }

    private void Move(Vector3 dir, float speed)
    {
        gameObject.transform.Translate(dir * speed * Time.deltaTime);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (rb_rigidbody != null && context.started)
        {
            // gameObject.transform.Translate(Vector3.up * JumpAmount * Time.deltaTime);
            rb_rigidbody.AddForce(Vector3.up * JumpAmount, ForceMode.Impulse);
        }
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        // not implementation
        if (context.started)
        {
            Debug.Log($"Left Click : Weapon use");
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        v_lookDirection = context.ReadValue<Vector2>();
    }
    private void Look(Vector2 dir)
    {
        transform.Rotate(Vector3.up * dir.x * MouseSensitivity * Time.deltaTime);
    }

    private void CameraRotation()
    {
        //Don't multiply mouse input by Time.deltaTime;
        float deltaTimeMultiplier = 1.0f;

        _cinemachineTargetYaw += v_lookDirection.x * deltaTimeMultiplier;
        _cinemachineTargetPitch += v_lookDirection.y * deltaTimeMultiplier;

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
