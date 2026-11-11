using Unity.Cinemachine;
using UnityEngine;

namespace Fish.Prototype
{
    /// <summary>
    /// Builds a Cinemachine over-the-shoulder camera at play time.
    /// The Follow target is expected to yaw and pitch (see AnglerMotor);
    /// Third Person Follow keeps the camera on the right shoulder and
    /// pulls in when the basin geometry occludes it.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class ShoulderCameraRig : MonoBehaviour
    {
        [SerializeField] Transform followTarget;
        [SerializeField] Vector3 shoulderOffset = new Vector3(0.48f, 0.1f, 0f);
        [SerializeField] float cameraDistance = 4.15f;
        [SerializeField] float verticalArmLength = 0.22f;
        [SerializeField] float fieldOfView = 52f;

        GameObject _vcamObject;

        void Awake()
        {
            if (followTarget == null)
            {
                var pivot = GameObject.Find("CameraPivot");
                if (pivot != null)
                    followTarget = pivot.transform;
            }

            if (GetComponent<CinemachineBrain>() == null)
                gameObject.AddComponent<CinemachineBrain>();

            if (followTarget == null)
            {
                Debug.LogError("ShoulderCameraRig needs a Follow target (CameraPivot).", this);
                return;
            }

            _vcamObject = new GameObject("CM Shoulder");
            var vcam = _vcamObject.AddComponent<CinemachineCamera>();
            vcam.Follow = followTarget;

            var lens = vcam.Lens;
            lens.FieldOfView = fieldOfView;
            lens.NearClipPlane = 0.15f;
            lens.FarClipPlane = 450f;
            vcam.Lens = lens;

            var body = _vcamObject.AddComponent<CinemachineThirdPersonFollow>();
            body.Damping = new Vector3(0.06f, 0.14f, 0.1f);
            body.ShoulderOffset = shoulderOffset;
            body.VerticalArmLength = verticalArmLength;
            body.CameraSide = 1f;
            body.CameraDistance = cameraDistance;
            body.AvoidObstacles = new CinemachineThirdPersonFollow.ObstacleSettings
            {
                Enabled = true,
                CollisionFilter = Physics.DefaultRaycastLayers,
                IgnoreTag = "Player",
                CameraRadius = 0.28f,
                DampingIntoCollision = 0.05f,
                DampingFromCollision = 0.25f
            };

            var aim = _vcamObject.AddComponent<CinemachineRotateWithFollowTarget>();
            aim.Damping = 0.02f;
        }

        void OnDestroy()
        {
            if (_vcamObject != null)
                Destroy(_vcamObject);
        }
    }
}
