using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerController : Entity
{
    #region Singleton and Awake
    public static PlayerController instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerController found!");
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(followCamera.gameObject);
        DontDestroyOnLoad(lockCamera.gameObject);

    }
    #endregion

    [Header("Components")]
    GameManager gameManager;
    public Camera cam;
    public Cinemachine.CinemachineVirtualCamera followCamera;
    public Cinemachine.CinemachineVirtualCamera lockCamera;
    public Rigidbody rb;
    public Collider coll;
    public Controls controls;
    public Animator anim;
    public Inventory inventory;


    public int baseDamage;
    [Header("Moving")]
    public Vector2 move;
    public Vector2 Local2DMovement;
    public float speedMultiplier;
    public bool _isMoving;
    public bool isGrounded;
    public float moveingDampTime;

    [Header("Smooth Turning")]
    public float turnSmoothVelocity;
    public float turnSmoothTime;
    public float turnSmoothDampTime;
    public float turnSmoothOffset;

    [Header("Animator")]
    public string vertical = "vertical";
    public string horizontal = "horizontal";

    [Header("Models")]
    public GameObject Helmet;
    public GameObject LeftHandParent;
    public GameObject RightHandParent;

    public string isLocked = "isLocked";
    public string isMoving = "isMoving";
    [Header("lock target mecanic")]
    bool _isLocked = false;
    GameObject actualEnemy = null;
    public Transform beetween;
    public float rotationSpeed = 2f;
    public UnityEngine.UI.Image lockImage;

    public enum States { free, atacking };
    public States actualState = States.free;
    [Header("Stats")]
    public Stat Damage;

    void isLockedChange()
    {
        if (_isLocked)
        {
            actualEnemy = null;
            lockCamera.gameObject.SetActive(false);
            _isLocked = false;
            anim.SetBool("isLocked", false);
        }
        else
        {
            anim.SetBool("isLocked", true);
            _isLocked = true;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        Damage = new(baseDamage);

        gameManager = GameManager.instance;
        inventory = Inventory.instance;


        controls = GameManager.instance.controls;
        //axes for moving
        controls.Player.Movement.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Player.Movement.canceled += ctx => move = Vector2.zero;

        //3 buttons for using assignable items (weapons, molotovs ,etc.)
        controls.Player.FirstActionButton.performed += ctx => UseActionButton(0);
        controls.Player.SecoundActionButton.performed += ctx => UseActionButton(1);
        controls.Player.ThirdActionButton.performed += ctx => UseActionButton(2);

    }


    // Update is called once per frame
    void Update()
    {

        CheckStatement();


        ChangeRotationAndMove();

    }

    private void UseActionButton(int buttonIndex)
    {
        if (AssignableItemsManager.instance.currentItems[buttonIndex] != null)
        {
            AssignableItemsManager.instance.currentItems[buttonIndex].UseInGame();
        }
    }

    //About CheckButtons() method:
    //It was used on early concept of locking camera at all enemies, now it's only used on bossess
    //It was inside Update() method

    /* private void CheckButtons()
     {
         *//*if (_isLocked)
         {
             if (actualEnemy == null)
             {
                 var enemyList = GameObject.FindGameObjectsWithTag("Enemy");
                 int distance = -1;
                 foreach (var enemy in enemyList)
                 {
                     var val = Vector3.Distance(transform.position, enemy.transform.position);
                     if (distance == -1)
                     {
                         distance = (int)val;
                         actualEnemy = enemy;
                     }
                     else
                     if (val < distance)
                     {
                         distance = (int)val;
                         actualEnemy = enemy;
                     }
                 }
             }
             lockImage.gameObject.SetActive(true);
             lockImage.transform.position = cam.WorldToScreenPoint(actualEnemy.transform.position);
             beetween.position = new(
                 Mathf.Lerp(actualEnemy.transform.position.x, transform.position.x, 0.5f),
                 Mathf.Lerp(actualEnemy.transform.position.y, transform.position.y, 0.5f),
                 Mathf.Lerp(actualEnemy.transform.position.z, transform.position.z, 0.5f)
                 );
             lockCamera.gameObject.SetActive(true);
         }
         else
         {
             lockImage.gameObject.SetActive(false);
             beetween.position = transform.position;
         }*//*
     }*/

    //CheckStatement() method is used to check if player is moving
    private void CheckStatement()
    {
        _isMoving = move.magnitude != 0;
    }
    private void ChangeRotationAndMove()
    {
        if (actualState == States.free)
        {

            //anim.SetFloat(horizontal, move.x, turnSmoothDampTime, Time.deltaTime);
            //anim.SetFloat(vertical, move.y, turnSmoothDampTime, Time.deltaTime);

            var deviation = /*Mathf.Abs(move.x) + Mathf.Abs(move.y);*/ move.magnitude;
            anim.SetFloat("WalkingDeviation", deviation, moveingDampTime, Time.deltaTime);
            if (_isMoving)
            {
                anim.SetBool(isMoving, true);
                Vector3 Direction = new(move.x, 0f, move.y);
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walk&Idle") || (anim.GetCurrentAnimatorStateInfo(0).IsName("SwordAndShieldWalk") && anim.GetCurrentAnimatorStateInfo(1).IsName("Nothing")))
                {
                    float targetAngle = Mathf.Atan2(Direction.x, Direction.z) * Mathf.Rad2Deg + cam.transform.rotation.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                    transform.rotation = Quaternion.Euler(0f, angle + turnSmoothOffset, 0f);
                    Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * deviation;
                    transform.Translate(speedMultiplier * Time.deltaTime * moveDir, Space.World);
                }

                if (_isLocked)
                {
                    //It makes strafing of a player synchronized with animation 
                    var localrt = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, transform.up) * new Vector3(move.x, 0, move.y);
                    Local2DMovement = new Vector2(localrt.x, localrt.z);

                    anim.SetFloat("S" + horizontal, Local2DMovement.x, turnSmoothDampTime, Time.deltaTime);
                    anim.SetFloat("S" + vertical, Local2DMovement.y, turnSmoothDampTime, Time.deltaTime);
                    var direction = (actualEnemy.transform.position - transform.position).normalized;
                    var rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);

                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotation.eulerAngles.y, transform.rotation.z);

                    //TODO: ADD DEVIATION
                    transform.Translate(speedMultiplier * Time.deltaTime * Direction, Space.World);

                }


                //transform.position += Direction * speed * Time.deltaTime;

            }
            else
            {
                anim.SetBool(isMoving, false);

            }
        }
    }
    public override void Die()
    {
        Debug.Log("Player Died. [*]");
        gameManager.EndGame();
    }
}
