using UnityEngine;
using UnityEngine.InputSystem;

namespace Fish.Prototype
{
    /// <summary>
    /// Third-person walk for the greybox pond. Mouse yaws the body and pitches
    /// <see cref="cameraPivot"/>; Cinemachine follows that pivot. Shift jogs.
    /// Short steps and slopes are handled by the CharacterController, not a climb.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class AnglerMotor : MonoBehaviour
    {
        [SerializeField] float walkSpeed = 2.2f;
        [SerializeField] float jogSpeed = 4.8f;
        [SerializeField] float lookSensitivity = 0.08f;
        [SerializeField] float minPitch = -42f;
        [SerializeField] float maxPitch = 55f;
        [SerializeField] Transform cameraPivot;
        [SerializeField] float fallResetHeight = 0.35f;

        CharacterController _controller;
        InputAction _move;
        InputAction _look;
        InputAction _jog;
        InputActionMap _ownedMap;
        float _pitch;
        float _verticalVelocity;
        Vector3 _spawnPosition;
        Quaternion _spawnRotation;

        void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (cameraPivot == null)
                cameraPivot = transform.Find("CameraPivot");

            _spawnPosition = transform.position;
            _spawnRotation = transform.rotation;
            BindInput();
        }

        void OnDestroy()
        {
            _ownedMap?.Dispose();
        }

        void Update()
        {
            UpdateCursor();
            if (Cursor.lockState == CursorLockMode.Locked)
                ApplyLook();
            ApplyMove();
            if (transform.position.y < fallResetHeight)
                Respawn();
        }

        void BindInput()
        {
            var project = InputSystem.actions;
            if (project != null)
            {
                // Project-wide actions ship with control-scheme groups. Leaving the
                // mask empty keeps keyboard and mouse live without a PlayerInput.
                project.bindingMask = null;
                project.FindActionMap("Player", false)?.Enable();
                _move = project.FindAction("Player/Move", false);
                _look = project.FindAction("Player/Look", false);
                _jog = project.FindAction("Player/Sprint", false);
            }

            if (_move == null || _look == null || _jog == null)
                CreateFallbackActions();

            _move.Enable();
            _look.Enable();
            _jog.Enable();
        }

        void CreateFallbackActions()
        {
            _ownedMap = new InputActionMap("PlayerFallback");
            _move = _ownedMap.AddAction("Move", InputActionType.Value, expectedControlType: "Vector2");
            _move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            _move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");

            _look = _ownedMap.AddAction("Look", InputActionType.Value, expectedControlType: "Vector2");
            _look.AddBinding("<Pointer>/delta");

            _jog = _ownedMap.AddAction("Jog", InputActionType.Button);
            _jog.AddBinding("<Keyboard>/leftShift");
        }

        void UpdateCursor()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }

            if (Application.isFocused
                && mouse != null
                && mouse.leftButton.wasPressedThisFrame
                && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        void ApplyLook()
        {
            if (_look == null)
                return;

            Vector2 look = _look.ReadValue<Vector2>();
            transform.Rotate(0f, look.x * lookSensitivity, 0f, Space.World);
            _pitch = Mathf.Clamp(_pitch - look.y * lookSensitivity, minPitch, maxPitch);
            if (cameraPivot != null)
                cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        void ApplyMove()
        {
            Vector2 move = _move != null ? _move.ReadValue<Vector2>() : Vector2.zero;
            Vector3 wish = transform.right * move.x + transform.forward * move.y;
            if (wish.sqrMagnitude < 0.0001f)
                wish = Vector3.zero;
            else if (wish.sqrMagnitude > 1f)
                wish.Normalize();

            bool jogging = _jog != null && _jog.IsPressed();
            float speed = jogging ? jogSpeed : walkSpeed;

            if (_controller.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;

            _verticalVelocity += Physics.gravity.y * Time.deltaTime;
            Vector3 motion = wish * speed;
            motion.y = _verticalVelocity;
            _controller.Move(motion * Time.deltaTime);
        }

        void Respawn()
        {
            _controller.enabled = false;
            transform.SetPositionAndRotation(_spawnPosition, _spawnRotation);
            _pitch = 0f;
            if (cameraPivot != null)
                cameraPivot.localRotation = Quaternion.identity;
            _verticalVelocity = 0f;
            _controller.enabled = true;
        }
    }
}
