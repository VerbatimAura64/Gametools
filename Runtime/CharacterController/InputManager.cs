using Harborview.GameTools;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
//using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;

namespace Harborview.GameTools
{
    [RequireComponent(typeof(CharacterController))]
    public class InputManager : MonoBehaviour
    {
        public IGameState gm;
        public IFragmentDisplay fm;
        public Camera mainCamera;
        public float interactRange = 1000f;
        public GameObject grabbedObject;
        public float distance;
        public GameObject thisModel;
        public float rotateSpeed = 10f;
        //public GameObject playerModel;
        protected GameObject gc;
        protected InputAction movement;
        protected InputAction jump;
        protected InputAction leftClick;
        protected InputAction rightClick;
        protected InputAction midClickHold;
        protected InputAction pause;
        protected InputAction next;
        protected InputAction sprint;
        protected InputAction restart;
        protected CharacterController cc;
        private Vector3 playerVelocity;
        protected Animator anim;
        protected LayerMask PlayerMask;
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
            PlayerMask = LayerMask.GetMask("Player");
            gc = GameObject.FindGameObjectWithTag("GameController");
            gm = gc.GetComponent<IGameState>();
            //thisModel = this.gameObject;
            //Debug.Log(gm == null);
            fm = gc.GetComponent<IFragmentDisplay>();
            //Debug.Log(fm == null);
            cc = GetComponent<CharacterController>();
            anim = GetComponentInChildren<Animator>();
            mainCamera = Camera.main;
            jump = InputSystem.actions.FindAction("Jump");
            movement = InputSystem.actions.FindAction("Move");
            movement = InputSystem.actions.FindAction("Move");
            leftClick = InputSystem.actions.FindAction("Shrink");
            rightClick = InputSystem.actions.FindAction("Grow");
            midClickHold = InputSystem.actions.FindAction("Drag");
            restart = InputSystem.actions.FindAction("Restart");
            //leftClickHold.AddBinding("<Mouse>/leftButton").WithInteraction("hold(duration=0.4)");
            //leftClickHold = new InputAction(binding: "<Keyboard>/leftButton", interactions: "hold(duration=.4)");
            pause = InputSystem.actions.FindAction("Pause");
            next = InputSystem.actions.FindAction("Next");
            sprint = InputSystem.actions.FindAction("Sprint");

            //leftClick.AddBinding
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }

        // Update is called once per frame
        void Update()
        {
            InputHandle();
            Rotation();
            anim.SetBool("Grounded", isGrounded);// = cc.isGrounded;
            
        }

        void FixedUpdate()
        {
            Movement();
            
            //Rotation();
        }

        void InputHandle()
        {
            Pause();
            Restart();
            Interact();
            Next();
            Jump();
        }

        void Restart()
        {
            if(restart.WasPressedThisFrame())
            {
                SceneManager.LoadScene(1);
            }
        }

        void Pause()
        {
            if (pause.WasPerformedThisFrame())
            {
                gm.PauseGame();
                Debug.Log("Pause");
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

        void Jump()
        {
            if (!gm.IsPaused)
            {
                if(isGrounded && jump.WasPerformedThisFrame())
                {
                    isGrounded = false;
                    playerVelocity.y = jumpHeight + playerVelocity.y;
                    anim.SetTrigger("Jump");
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
                //anim.SetBool("Walking", true);
                //anim.SetFloat("walkingY", moveY, dampTime, Time.deltaTime);
                //anim.SetFloat("walkingX", moveX, dampTime, Time.deltaTime);



                if (moveValue != Vector2.zero)
                {
                    anim.SetBool("Walking", true);
                    float targetAngle = Mathf.Atan2(moveValue.x, moveValue.y) * Mathf.Rad2Deg;
                    thisModel.transform.localRotation = Quaternion.Slerp(thisModel.transform.localRotation, Quaternion.Euler(0, targetAngle, 0), Time.deltaTime * rotateSpeed);
                }

                else
                    anim.SetBool("Walking", false);

               




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
            if (!gm.IsPaused)
            {
                Vector2 mousePos = Mouse.current.delta.ReadValue();
                rotationX += mousePos.y * -1 * sensitivity;
                rotationY += mousePos.x * sensitivity;

                rotationX = Mathf.Clamp(rotationX, -40f, 40f);
                transform.localEulerAngles = new Vector3(0f, rotationY, 0f);
                mainCamera.transform.localEulerAngles = new Vector3(rotationX, 0f, 0f);
            }
        }

        void Interact()
        {
            if (!gm.IsPaused)
            {
                
                Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                //Vector2 mousePos = Mouse.current.position.ReadValue();
                //Debug.Log("Mouse Position: " + mousePos);
                //leftClick.started +=;
                if (leftClick.WasPressedThisFrame())
                //if(leftClickHold.performed)
                {
                    //Ray ray = mainCamera.ScreenPointToRay(mousePos);
                    Ray ray = mainCamera.ScreenPointToRay(screenCenter);//RESOLVED: Can no longer use Input.mousePosition, need to use InputSystem instead

                    if (Physics.Raycast(ray, out RaycastHit hit, interactRange, ~PlayerMask))
                    {
                        Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);
                        Interactable target = hit.collider.GetComponent<Interactable>();
                        if (target != null)
                        {
                            target.Grow();
                            anim.SetTrigger("Attack");
                            //target.ReturnAnswer();
                        }
                    }
                    //Debug.Log("Attack");
                    
                }
                if (rightClick.WasPressedThisFrame())
                {
                    //Ray ray = mainCamera.ScreenPointToRay(mousePos);
                    Ray ray = mainCamera.ScreenPointToRay(screenCenter);//RESOLVED: Can no longer use Input.mousePosition, need to use InputSystem instead

                    if (Physics.Raycast(ray, out RaycastHit hit, interactRange, ~PlayerMask))
                    {
                        Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);
                        Interactable target = hit.collider.GetComponent<Interactable>();
                        if (target != null)
                        {
                            if (!target.WillsHeart)
                            {
                                target.Shrink();
                                anim.SetTrigger("Attack");
                            }//target.ReturnAnswer();
                        }
                    }
                    //Debug.Log("Attack");
                    
                }
                //anim.SetBool("Attack", false);
                if (midClickHold.IsPressed())
                {
                    Ray ray = mainCamera.ScreenPointToRay(screenCenter);
                    
                    if (Physics.Raycast(ray, out RaycastHit hit, interactRange, ~PlayerMask))
                    {
                        Debug.DrawLine(ray.origin, hit.point, Color.red, 1f);
                        Interactable target = hit.collider.GetComponent<Interactable>();
                        if (midClickHold.WasPressedThisFrame())
                        {
                            if (target != null) { 
                                grabbedObject = hit.collider.gameObject;

                                distance = Vector3.Distance(ray.origin, grabbedObject.transform.position);
                                Debug.Log(distance);
                            }
                        }
                        
                        //Physics.Raycast(ray, out  hit, distance, ~PlayerMask);
                        
                        
                    }
                    if (grabbedObject != null)
                        {
                            grabbedObject.transform.position = ray.GetPoint(distance);
                        }
                    
                    
                }
                if (midClickHold.WasReleasedThisFrame())
                {
                    grabbedObject = null;
                    distance = 0f;
                }
            }
        }

        private void LeftClick_started(InputAction.CallbackContext obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
