using UnityEngine;
using UnityEngine.InputSystem;
using EventBusSystem;
using System;

namespace Player
{
    public class MoveController : MonoBehaviour, IMoveSubscriber
    {
        [SerializeField] private float speed;
        //[SerializeField] private GameObject camera;

        Animator animator;
        Rigidbody rb;
        Vector3 moveDirection;
        LookDirection lookDirection = LookDirection.Right;
        CordinateSide cordinateSide = CordinateSide.XBack;
        IInteractive interactiveObject;


        InputAction moveNow = new InputAction();
        InputAction moveActionXFront;
        InputAction moveActionXBack;
        InputAction moveActionZFront;
        InputAction moveActionZBack;
        InputAction ladderMove;

        InputAction interact;

        SpriteRenderer sprite;
        bool isFliped;
        bool isGrounded;
        void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            sprite = GetComponent<SpriteRenderer>();
            moveActionXFront = InputSystem.actions.FindAction("MoveXFront");
            moveActionXBack = InputSystem.actions.FindAction("MoveXBack");
            moveActionZFront = InputSystem.actions.FindAction("MoveZFront");
            moveActionZBack = InputSystem.actions.FindAction("MoveZBack");
            ladderMove = InputSystem.actions.FindAction("LadderMove");

            moveNow = moveActionXBack;
            interact = InputSystem.actions.FindAction("Interact");
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
            //Debug.Log($"{moveNow.ReadValue<Vector3>()} for {cordinateSide}");

            if ((moveNow.ReadValue<Vector3>() == new Vector3(0, 0, -1) && cordinateSide == CordinateSide.XBack)
                | (moveNow.ReadValue<Vector3>() == new Vector3(0, 0, 1) && cordinateSide == CordinateSide.XFront)
                | (moveNow.ReadValue<Vector3>() == new Vector3(1, 0, 0) && cordinateSide == CordinateSide.ZBack)
                | (moveNow.ReadValue<Vector3>() == new Vector3(-1, 0, 0) && cordinateSide == CordinateSide.ZFront) )
            {
                ChangeLookDiraction(LookDirection.Right);
            }
            else if ((moveNow.ReadValue<Vector3>() == new Vector3(1, 0, 0) && cordinateSide == CordinateSide.ZFront)
                | (moveNow.ReadValue<Vector3>() == new Vector3(0, 0, 1) && cordinateSide == CordinateSide.XBack)
                | (moveNow.ReadValue<Vector3>() == new Vector3(0, 0, -1) && cordinateSide == CordinateSide.XFront)
                | (moveNow.ReadValue<Vector3>() == new Vector3(-1, 0, 0) && cordinateSide == CordinateSide.ZBack))
            {
                ChangeLookDiraction(LookDirection.Left);
            }
            if (interact.WasPressedThisFrame())
            {
                interactiveObject.Interact();
            }
        }

        private void FixedUpdate()
        {
            moveDirection = moveNow.ReadValue<Vector3>();
            rb.linearVelocity = speed * Time.fixedDeltaTime * moveDirection;
            
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

        public void ChangePlayerSide(CordinateSide side)
        {
            cordinateSide = side;

            switch(cordinateSide)
            {
                case CordinateSide.XBack:
                    moveNow = moveActionXBack;
                    transform.eulerAngles = new Vector3(0, 90, 0);
                    break;
                case CordinateSide.ZBack:
                    moveNow = moveActionZBack;
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    break;
                case CordinateSide.ZFront:
                    moveNow = moveActionZFront;
                    transform.eulerAngles = new Vector3(0, 180, 0);
                    break;
                case CordinateSide.XFront:
                    moveNow = moveActionXFront;
                    transform.eulerAngles = new Vector3(0, 270, 0);
                    break;
                default:
                    break;
            }
        }

        public void ChangeLookDiraction(LookDirection direction)
        {
            lookDirection = direction;
            if (lookDirection == LookDirection.Left)
            {
                isFliped = false;
                sprite.flipX = !isFliped;

            }
            else if (lookDirection == LookDirection.Right)
            {
                isFliped = true;
                sprite.flipX = !isFliped;
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
    }
}
