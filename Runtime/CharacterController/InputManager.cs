using UnityEngine;
using UnityEngine.InputSystem;
using Harborview.GameTools;

namespace Harborview.GameTools
{
    [RequireComponent(typeof(CharacterController))]
    public class InputManager : MonoBehaviour
    {
        public IGameState gm;
        public IFragmentDisplay fm;
        public Camera mainCamera;
        public float interactRange = 1000f;
        public GameObject gc;
        protected InputAction movement;
        protected InputAction click;
        protected InputAction pause;
        protected InputAction next;
        protected InputAction sprint;
        protected CharacterController cc;
        private Vector3 playerVelocity;
        protected Animator anim;
        [SerializeField] private bool isGrounded;

        [SerializeField] private float playerSpeed = 5f;
        [SerializeField] private float sprintMultiplier = 2f;
        [SerializeField] private float dampTime = 1f;
        [SerializeField] private float jumpHeight = 1f;
        [SerializeField] private float gravity = -9.81f;
        public float rotationX;
        public float rotationY;
        public float sensitivity = .02f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            gc = GameObject.FindGameObjectWithTag("GameController");
            gm = gc.GetComponent<IGameState>();
            //Debug.Log(gm == null);
            fm = gc.GetComponent<IFragmentDisplay>();
            //Debug.Log(fm == null);
            cc = GetComponent<CharacterController>();
            //anim = GetComponent<Animator>();
            mainCamera = Camera.main;
            movement = InputSystem.actions.FindAction("Move");
            click = InputSystem.actions.FindAction("Attack");
            pause = InputSystem.actions.FindAction("Pause");
            next = InputSystem.actions.FindAction("Next");
            sprint = InputSystem.actions.FindAction("Sprint");
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        // Update is called once per frame
        void Update()
        {
            Rotation();
            //isGrounded = cc.isGrounded;
            InputHandle();
        }

        void FixedUpdate()
        {
            Movement();
            //Rotation();
        }

        void InputHandle()
        {
            Pause();
            Interact();
            Next();
        }

        void Pause()
        {
            if (pause.WasPerformedThisFrame())
            {
                gm.PauseGame();
            }
        }

        void Next()
        {
            if (next.WasPerformedThisFrame())
            {
                if (fm.Panel.activeInHierarchy)
                {
                    fm.ClosePanel();
                }
            }
        }

        void Movement()
        {
            if (!gm.IsPaused)
            {
                isGrounded = cc.isGrounded;
                if (isGrounded && playerVelocity.y < 0)
                {
                    playerVelocity.y = 0f;
                }
                Vector2 moveValue = movement.ReadValue<Vector2>();
                float moveX = moveValue.x;
                float moveY = moveValue.y;
                Vector3 move = transform.right * moveX + transform.forward * moveY;
                //Debug.Log(moveValue);
                float currentSpeed = sprint.IsPressed() ? playerSpeed * sprintMultiplier : playerSpeed;
                cc.Move(move * Time.deltaTime * currentSpeed);
                //cc.Move(move * Time.deltaTime * playerSpeed);
                playerVelocity.y += gravity * Time.deltaTime;
                cc.Move(playerVelocity * Time.deltaTime);
                //anim.SetFloat("walkingY", moveY, dampTime, Time.deltaTime);
                //anim.SetFloat("walkingX", moveX, dampTime, Time.deltaTime);

                /*if(moveValue != Vector2.zero)
                    anim.SetBool("isWalking", true);

                else
                    anim.SetBool("isWalking", false);
                */



                //float time = Mathf.PingPong(Time.time, 1);
                //if (moveValue == Vector2.zero)
                {
                    //              anim.SetFloat("isWalking", 0f);
                }
                //        else
                {
                    //          anim.SetFloat("isWalking", 1f);
                }

                //anim.SetFloat("isWalking", moveValue.x);
                //Debug.Log("Moving: " + moveValue);
                //rb.MovePosition(rb.position + new Vector3(moveValue.x, 0, moveValue.y) * time);

            }
        }


        void Rotation()
        {
            Vector2 mousePos = Mouse.current.delta.ReadValue();
            rotationX += mousePos.y * -1 * sensitivity;
            rotationY += mousePos.x * sensitivity;

            rotationX = Mathf.Clamp(rotationX, -40f, 40f);
            transform.localEulerAngles = new Vector3(0f, rotationY, 0f);
            mainCamera.transform.localEulerAngles = new Vector3(rotationX, 0f, 0f);
        }

        void Interact()
        {
            if (!gm.IsPaused)
            {
               //Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                Vector2 mousePos = Mouse.current.position.ReadValue();
                //Debug.Log("Mouse Position: " + mousePos);
                if (click.WasPerformedThisFrame())
                {
                    Ray ray = mainCamera.ScreenPointToRay(mousePos);
                   // Ray ray = mainCamera.ScreenPointToRay(screenCenter);//RESOLVED: Can no longer use Input.mousePosition, need to use InputSystem instead

                    if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
                    {
                        Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);
                        Interactable target = hit.collider.GetComponent<Interactable>();
                        if (target != null)
                        {
                            target.ReturnAnswer();
                        }
                    }
                    //Debug.Log("Attack");
                }
            }
        }
    }
}
