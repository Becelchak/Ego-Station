using UnityEngine;
using UnityEngine.InputSystem;
using EventBusSystem;
using Debug = UnityEngine.Debug;

namespace Player
{
    public class MoveController : MonoBehaviour, IMoveControllerSubscriber
    {
        [SerializeField] private Transform feetTrigger;
        //[SerializeField] private LayerMask groundLayer;   // Слой для проверки пола
        //private float maxStepHeight = 0.5f; // Максимальная высота подъема
        //private float stepDuration = 0.2f;  // Время подъема
        //private float stepTimer = 0f;       // Таймер подъема
        //private bool isStepping = false;    // Флаг подъема
        //private float stepTargetY;          // Целевая высота подъема

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

            // Проверка на перепады высот
            if (Mathf.Abs(moveDirection.x) > 0)
            {
                CheckForStep(moveDirection.x);
            }
        }

        private void CheckForStep(float direction)
        {
            //// Начальная точка Raycast (глобальные координаты триггера ног)
            //Vector2 rayOrigin = feetTrigger.position;
            //Vector2 rayDirection = Vector2.right * direction;

            //// Длина Raycast
            //float rayLength = 1f;

            //// Отладочная визуализация
            //Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.red, 1f);

            //// Выполняем Raycast
            //RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayLength, groundLayer);

            //if (hit.collider != null)
            //{
            //    // Если игрок упирается в препятствие, начинаем подъем
            //    if (!isStepping)
            //    {
            //        StartStep(hit.point.y);
            //    }
            //}
            //else
            //{
            //    // Если препятствий нет, сбрасываем подъем
            //    isStepping = false;
            //}

            //// Если игрок поднимается, обновляем его позицию
            //if (isStepping)
            //{
            //    UpdateStep();
            //}
        }

        private void StartStep(float targetY)
        {
            //// Начинаем подъем
            //isStepping = true;
            //stepTimer = 0f;

            //// Ограничиваем максимальную высоту подъема
            //stepTargetY = Mathf.Min(targetY, transform.position.y + maxStepHeight);
        }

        private void UpdateStep()
        {
            //// Увеличиваем таймер
            //stepTimer += Time.deltaTime;

            //// Если время подъема истекло, завершаем подъем
            //if (stepTimer >= stepDuration)
            //{
            //    isStepping = false;
            //    rb.linearVelocity = new Vector2(rb.linearVelocityX, 0); // Сбрасываем скорость по Y
            //    return;
            //}

            //// Вычисляем целевую скорость для подъема
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
