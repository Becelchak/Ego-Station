using UnityEngine;
using UnityEngine.InputSystem;
using EventBusSystem;
using Debug = UnityEngine.Debug;

namespace Player
{
    public class MoveController : MonoBehaviour, IMoveControllerSubscriber
    {
        [SerializeField] private Transform feetTrigger;
        //[SerializeField] private LayerMask groundLayer;   // ���� ��� �������� ����
        //private float maxStepHeight = 0.5f; // ������������ ������ �������
        //private float stepDuration = 0.2f;  // ����� �������
        //private float stepTimer = 0f;       // ������ �������
        //private bool isStepping = false;    // ���� �������
        //private float stepTargetY;          // ������� ������ �������

        [SerializeField] private float speed;
        Animator animator;
        Rigidbody2D rb;
        Vector3 moveDirection;
        LookDirection lookDirection = LookDirection.Left;
        IInteractive interactiveObject;


        InputAction moveNow = new InputAction();
        InputAction horizontalMove;
        InputAction ladderMove;

        InputAction interact;
        bool isGrounded;
        bool isFreezed;
        void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            horizontalMove = InputSystem.actions.FindAction("MoveZBack");
            ladderMove = InputSystem.actions.FindAction("LadderMove");

            moveNow = horizontalMove;
            interact = InputSystem.actions.FindAction("Interact");

            moveNow.performed += OnMovePerformed;
            moveNow.canceled += OnMoveCanceled;
        }

        private void OnDestroy()
        {
            moveNow.performed -= OnMovePerformed;
            moveNow.canceled -= OnMoveCanceled;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            if (!isFreezed)
            {
                animator.SetBool("IsMoving", true);
                animator.Play("Move", -1, 0f);
            }
            
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            animator.SetBool("IsMoving", false);
            animator.Play("Idle", -1, 0f);
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

            if (moveNow.ReadValue<Vector3>() == new Vector3(1, 0, 0) )
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
            rb.linearVelocity = new Vector2(moveDirection.x * speed * Time.fixedDeltaTime, rb.linearVelocity.y);

            // �������� �� �������� �����
            if (Mathf.Abs(moveDirection.x) > 0)
            {
                CheckForStep(moveDirection.x);
            }
        }

        private void CheckForStep(float direction)
        {
            //// ��������� ����� Raycast (���������� ���������� �������� ���)
            //Vector2 rayOrigin = feetTrigger.position;
            //Vector2 rayDirection = Vector2.right * direction;

            //// ����� Raycast
            //float rayLength = 1f;

            //// ���������� ������������
            //Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red, 1f);

            //// ��������� Raycast
            //RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, groundLayer);

            //if (hit.collider != null)
            //{
            //    // ���� ����� ��������� � �����������, �������� ������
            //    if (!isStepping)
            //    {
            //        StartStep(hit.point.y);
            //    }
            //}
            //else
            //{
            //    // ���� ����������� ���, ���������� ������
            //    isStepping = false;
            //}

            //// ���� ����� �����������, ��������� ��� �������
            //if (isStepping)
            //{
            //    UpdateStep();
            //}
        }

        private void StartStep(float targetY)
        {
            //// �������� ������
            //isStepping = true;
            //stepTimer = 0f;

            //// ������������ ������������ ������ �������
            //stepTargetY = Mathf.Min(targetY, transform.position.y + maxStepHeight);
        }

        private void UpdateStep()
        {
            //// ����������� ������
            //stepTimer += Time.deltaTime;

            //// ���� ����� ������� �������, ��������� ������
            //if (stepTimer >= stepDuration)
            //{
            //    isStepping = false;
            //    rb.linearVelocity = new Vector2(rb.linearVelocityX, 0); // ���������� �������� �� Y
            //    return;
            //}

            //// ��������� ������� �������� ��� �������
            //float targetVelocity = (stepTargetY - transform.position.y) / (stepDuration - stepTimer);
            //rb.linearVelocity = new Vector2(rb.linearVelocityX, targetVelocity);
        }

        public enum CordinateSide
        {
            XFront,
            XBack,
            ZFront,
            ZBack,
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
                transform.localScale = new Vector3(1,1,1);

            }
            else if (lookDirection == LookDirection.Right)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        public void SetNewInteractiveObject(IInteractive newInteractive)
        {
            interactiveObject = newInteractive;
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
