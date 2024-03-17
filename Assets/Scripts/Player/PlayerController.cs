using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Windows;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player")]
        public float moveSpeed = 4.0f;
        public float sprintSpeed = 6.0f;
        public float rotationSpeed = 1.0f;
        public float speedChangeRate = 10.0f;
        public bool IsSprint { get { return GameInput.Instance.IsSprint(); } private set { } }

        [Space(10)]
        [SerializeField] private float _jumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        [SerializeField] private float gravity = -15.0f;

        [Space(10)]
        public float jumpTimeout = 0.1f;
        public float fallTimeout = 0.15f;

        [Header("Player Grounded")]
        [SerializeField] private bool _grounded = true;
        [SerializeField] private float _groundedOffset = -0.14f;
        [SerializeField] private float _groundedRadius = 0.5f;
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        public GameObject cinemachineCameraTarget;
        public float mouseSensitivity = 0.3f;
        [SerializeField] private float _topClamp = 89.0f;
        [SerializeField] private float _bottomClamp = -89.0f;

        // Cinemachine
        private float _cinemachineTargetPitch;

        // Player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // Timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // Animation Status
        private float _animationMoveSpeed;
        private bool _isJumping;
        private bool _isFreeFall;

        private CinemachineVirtualCamera _cinemachineVirtualCamera;
        private PlayerInput _playerInput;
        private CharacterController _controller;
        private GameObject _mainCamera;
        private Animator _animator;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
                return _playerInput.currentControlScheme == "KeyboardAndMouse";
            }
        }

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            _cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
            _cinemachineVirtualCamera.Follow = cinemachineCameraTarget.transform;

            // reset our timeouts on start
            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;
        }

        private void Update()
        {
            JumpAndGravity();
            _grounded = _controller.isGrounded;
            //GroundedCheck();
            Move();
            //Debug.Log(IsSprint);
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void OnApplicationFocus(bool focus)
        {
            Cursor.visible = focus;
            Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
        }

        private void GroundedCheck()
        {
            // Set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
            _grounded = Physics.CheckSphere(spherePosition, _groundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            Vector2 lookInput = GameInput.Instance.GetLookVector();
            if (lookInput.sqrMagnitude >= _threshold)
            {
                //Don't multiply mouse input by Time.deltaTime
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? mouseSensitivity : Time.deltaTime;

                _cinemachineTargetPitch += lookInput.y * rotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = lookInput.x * rotationSpeed * deltaTimeMultiplier;

                // Sýnýrlandýr
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);

                cinemachineCameraTarget.transform.localRotation = Quaternion.Euler(-_cinemachineTargetPitch, 0.0f, 0.0f);

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = GameInput.Instance.IsSprint() ? sprintSpeed : moveSpeed;
            //float targetSpeed = moveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            Vector2 moveInput = GameInput.Instance.GetMovementVectorNormalized();
            if (moveInput == Vector2.zero)
                targetSpeed = 0.0f;

            _animationMoveSpeed = targetSpeed;

            if (moveInput.y < 0f)
                _animationMoveSpeed = -targetSpeed;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            //float inputMagnitude = _input.analogMovement ? moveInput.magnitude : 1f;
            float inputMagnitude = 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (moveInput != Vector2.zero)
            {
                // move
                inputDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
            }

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (_grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = fallTimeout;

                _isJumping = false;
                _isFreeFall = false;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (GameInput.Instance.isJump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * gravity);

                    _isJumping = true;
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = jumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    _isFreeFall = true;
                }

                // if we are not grounded, do not jump
                GameInput.Instance.isJump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (_grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z), _groundedRadius);
        }

        public bool IsGrounded()
        {
            return _grounded;
        }

        public bool IsJumping()
        {
            return _isJumping;
        }

        public bool IsFreeFall()
        {
            return _isFreeFall;
        }

        public float GetSpeed()
        {
            return _animationMoveSpeed;
        }
    }
}

