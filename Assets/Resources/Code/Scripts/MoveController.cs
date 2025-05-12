using UnityEngine;
using UnityEngine.InputSystem;
using EventBusSystem;
using Debug = UnityEngine.Debug;

namespace Player
{
    public class MoveController : MonoBehaviour, IMoveControllerSubscriber
    {
        [SerializeField] private Transform feetTrigger;
        [SerializeField] private float speed;
        [SerializeField] private InteractionLabelController labelController;
        [SerializeField] private AudioClip walkingSound;
        [SerializeField] private AudioClip ladderSound;

        InputAction moveNow = new InputAction();
        InputAction horizontalMove;
        InputAction ladderMove;
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
        private bool wasMoving;

        
        void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            playerAudioSource = GetComponent<AudioSource>();
            horizontalMove = InputSystem.actions.FindAction("MoveZBack");
            ladderMove = InputSystem.actions.FindAction("LadderMove");
            interact = InputSystem.actions.FindAction("Interact");

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
            if (isFreezed) return;

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
            if (isOnLadder)
            {
                rb.linearVelocity = new Vector2(0, moveDirection.y * speed * Time.fixedDeltaTime);
            }
            else
                rb.linearVelocity = new Vector2(moveDirection.x * speed * Time.fixedDeltaTime, rb.linearVelocity.y);

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
        }

        public void Unfreeze()
        {
            isFreezed = false;
        }
    }
}