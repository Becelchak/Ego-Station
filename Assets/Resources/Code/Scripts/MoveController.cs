using UnityEngine;
using UnityEngine.InputSystem;
using EventBusSystem;
using Debug = UnityEngine.Debug;
using UnityEngine.U2D.Animation;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Player
{
    public class MoveController : MonoBehaviour, IMoveControllerSubscriber, INoControllRotateSubscriber
    {
        [SerializeField] private Transform feetTrigger;
        [SerializeField] private float speed;
        [SerializeField] private InteractionLabelController labelController;
        [SerializeField] private AudioClip walkingSound;
        [SerializeField] private AudioClip ladderSound;

        [Header("Zero Gravity Settings")]
        [SerializeField] private float zeroGravityDrag = 0.5f;
        [SerializeField] private float normalGravityScale = 1f;
        [SerializeField] private float maxZeroGSpeed = 5f;
        [SerializeField] private float rotationSmoothTime = 1.0f;
        [SerializeField] private GameObject modelWithSkins;
        private bool inZeroGravity;
        private NoGravityZone currentGravityZone;
        private float originalDrag;
        private List<SpriteSkin> spriteSkins;
        private Coroutine rotationCoroutine;
        [Header("Rotation Settings")]
        [SerializeField] private float rotationStopSmoothing = 2f;
        private bool isRotating;
        private float currentRotationSpeed;

        InputAction moveNow = new InputAction();
        InputAction horizontalMove;
        InputAction ladderMove;
        InputAction noGravityMove;

        Animator animator;
        Rigidbody2D rb;
        Vector3 moveDirection;
        LookDirection lookDirection = LookDirection.Left;
        IInteractive interactiveObject;

        InputAction interact;
        //bool isGrounded;
        bool isFreezed;
        private bool isOnLadder;
        private AudioSource playerAudioSource;
        //private bool wasMoving;

        
        void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            playerAudioSource = GetComponent<AudioSource>();
            horizontalMove = InputSystem.actions.FindAction("MoveZBack");
            ladderMove = InputSystem.actions.FindAction("LadderMove");
            interact = InputSystem.actions.FindAction("Interact");
            noGravityMove = InputSystem.actions.FindAction("NoGravityMove");

            horizontalMove.performed += OnMovePerformed;
            horizontalMove.canceled += OnMoveCanceled;
            ladderMove.performed += OnMovePerformed;
            ladderMove.canceled += OnMoveCanceled;

            moveNow = horizontalMove;

            if (walkingSound == null)
                walkingSound = Resources.Load<AudioClip>("Audio/Sound/SFX/Footsteps");
            if (ladderSound == null)
                ladderSound = Resources.Load<AudioClip>("Audio/Sound/SFX/Metalic_ladder_move");
        }

        private void OnDestroy()
        {
            moveNow.performed -= OnMovePerformed;
            moveNow.canceled -= OnMoveCanceled;
            ladderMove.performed -= OnMovePerformed;
            ladderMove.canceled -= OnMoveCanceled;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            if (isFreezed || inZeroGravity) return;

            animator.SetBool("IsMoving", true);
            if (isOnLadder)
            {
                animator.SetBool("IsClimbing", true);
                animator.CrossFade("Climb", 0.25f, -1, 0f, 0.5f);
                PlayMovementSound(ladderSound);
            }
            else
            {
                animator.SetBool("IsClimbing", false);
                animator.Play("Move", -1, 0f);
                PlayMovementSound(walkingSound);
            }

        }

        private void PlayMovementSound(AudioClip clip)
        {
            if (playerAudioSource.isPlaying && playerAudioSource.clip == clip)
                return;

            playerAudioSource.clip = clip;
            playerAudioSource.Play();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            animator.SetBool("IsMoving", false);
            playerAudioSource.Stop();

            if (isOnLadder)
            {
                animator.Play("ClimbIdle", -1, 0f);
            }
            else
            {
                animator.Play("Idle", -1, 0f);
            }
        }

        private void OnEnable()
        {
            Debug.Log("MoveController: Subscribing to EventBus");
            EventBus.Subscribe(this);
        }

        private void OnDisable()
        {
            Debug.Log("MoveController: Unsubscribing to EventBus");
            EventBus.Unsubscribe(this);
        }

        void Update()
        {
            if (isFreezed)
                return;

            if (moveNow.ReadValue<Vector3>() == new Vector3(1, 0, 0))
            {
                ChangeLookDiraction(LookDirection.Right);
            }
            else if (moveNow.ReadValue<Vector3>() == new Vector3(-1, 0, 0))
            {
                ChangeLookDiraction(LookDirection.Left);
            }

            if (interact.WasPressedThisFrame() && interactiveObject != null)
            {
                interactiveObject.Interact();
            }
        }

        private void FixedUpdate()
        {
            if (isFreezed)
                return;

            moveDirection = moveNow.ReadValue<Vector3>();

            if (inZeroGravity)
            {
                HandleZeroGravityMovement();
                return;
            }
            else if(isOnLadder)
            {
                rb.linearVelocity = new Vector2(0, moveDirection.y * speed * Time.fixedDeltaTime);
            }
            else
                rb.linearVelocity = new Vector2(moveDirection.x * speed * Time.fixedDeltaTime, rb.linearVelocity.y);

        }

        private void HandleZeroGravityMovement()
        {
            float moveInput = moveDirection.y;
            float rotateInput = moveDirection.x;

            if (Mathf.Abs(rotateInput) > 0.1f)
            {
                float rotationAmount = rotateInput * currentGravityZone.GetRotationSpeed() * Time.fixedDeltaTime;
                transform.Rotate(0, 0, -rotationAmount);
            }

            if (Mathf.Abs(moveInput) > 0.1f)
            {
                Vector2 moveDirection = transform.up * moveInput;

                rb.AddForce(moveDirection * currentGravityZone.GetImpulseForce(),
                           ForceMode2D.Impulse);
            }

            if (rb.linearVelocity.magnitude > maxZeroGSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxZeroGSpeed;
            }
        }

        public void OnEnterZeroGravity(NoGravityZone zone)
        {
            inZeroGravity = true;
            currentGravityZone = zone;
            rb.gravityScale = 0;
            rb.linearDamping = zeroGravityDrag;
            animator.enabled = false;
            moveNow = noGravityMove;

            spriteSkins = modelWithSkins.GetComponentsInChildren<SpriteSkin>().ToList();
            foreach (var spriteSkin in spriteSkins) 
            {
                //if (spriteSkin != null) spriteSkin.enabled = true;
            }
            
        }

        public void OnExitZeroGravity()
        {
            inZeroGravity = false;
            currentGravityZone = null;
            rb.gravityScale = normalGravityScale;
            rb.linearDamping = originalDrag;
            animator.enabled = true;
            moveNow = horizontalMove;

            if (rotationCoroutine != null)
            {
                StopCoroutine(rotationCoroutine);
            }

            rotationCoroutine = StartCoroutine(SmoothRotationToDefault());
            foreach (var spriteSkin in spriteSkins)
            {
                //if (spriteSkin != null) spriteSkin.enabled = false;
            }
        }

        private IEnumerator SmoothRotationToDefault()
        {
            var startRotation = transform.rotation;
            var targetRotation = Quaternion.Euler(0, 0, 0);
            var elapsedTime = 0f;

            while (elapsedTime < rotationSmoothTime)
            {
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation,
                    elapsedTime / rotationSmoothTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.rotation = targetRotation;
            rotationCoroutine = null;

        }

        public enum LookDirection
        {
            Left,
            Right,
        }

        public void ChangeLookDiraction(LookDirection direction)
        {
            lookDirection = direction;
            if (lookDirection == LookDirection.Left)
            {
                transform.localScale = new Vector3(1, 1, 1);

            }
            else if (lookDirection == LookDirection.Right)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        public void SetNewInteractiveObject(IInteractive newInteractive)
        {
            if (interactiveObject == newInteractive)
                return;

            if (interactiveObject != null && labelController != null)
            {
                labelController.HideLabel();
            }

            interactiveObject = newInteractive;

            if (interactiveObject != null && labelController != null)
            {
                labelController.ShowLabel(interactiveObject, transform.position);
            }
        }

        public void StartClimbing()
        {
            isOnLadder = true;
            moveNow = ladderMove;
            rb.gravityScale = 0;
            playerAudioSource.Stop();
            PlayMovementSound(ladderSound);
            animator.SetBool("IsClimbing", true);
        }

        public void EndClimbing()
        {
            isOnLadder = false;
            moveNow = horizontalMove;
            rb.gravityScale = 1;
            playerAudioSource.Stop();
            PlayMovementSound(walkingSound);
            animator.SetBool("IsClimbing", false);
        }

        public bool OnGround()
        {
            return true;
        }

        public void Freeze()
        {
            isFreezed = true;
            var force = new Vector2(0, 0);
            rb.totalForce = force;
        }

        public void Unfreeze()
        {
            isFreezed = false;
        }

        public void OnApplyRandomRotation(float rotationForce, float duration)
        {
            // Останавливаем предыдущее вращение если было
            if (rotationCoroutine != null)
            {
                StopCoroutine(rotationCoroutine);
            }

            // Применяем вращение
            rotationCoroutine = StartCoroutine(ApplyRotationEffect(rotationForce, duration));
        }

        private IEnumerator ApplyRotationEffect(float rotationForce, float duration)
        {
            isRotating = true;
            currentRotationSpeed = speed;
            float elapsed = 0f;

            Debug.Log("Start rotate");
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);
                yield return null;
            }

            Debug.Log("End rotate");

            StopCoroutine(rotationCoroutine);
            StartCoroutine(SmoothRotationToDefault());

            transform.rotation = Quaternion.identity;
            isRotating = false;
        }

    }
}