using UnityEditor.UIElements;
using UnityEngine;

[CreateAssetMenu(fileName = "CinemachineCameraData", menuName = "Scriptable Objects/CinemachineCameraData")]
public class CinemachineCameraData : ScriptableObject
{
    public Vector3 damping;
    public Vector3 shoulderOffset;
    public float verticalArmLength;
    public float cameraSide;
    public float cameraDistance;
    public LayerMask cameraCollisionFilter;
    public string ignoreTag;
    public float cameraRadius;
}
