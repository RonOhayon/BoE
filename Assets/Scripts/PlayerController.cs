using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private int speed;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private LayerMask grassLayer;
    [SerializeField] private int stepsInGrass;
    [SerializeField] private int minStepToEncounter;
    [SerializeField] private int maxStepToEncounter;

    private PlayerControls playerControl;
    private Rigidbody rb;
    private Vector3 movement;
    private bool movingInGrass;
    private float stepTimer;
    private int stepToEncounter;
    private PartyManager partManager;



    private const string IS_WALK_PARAM = "IsWalk";
    private const string BATTLE_SCENE = "BattleScene";
    private const float TIME_PER_STEP = 0.5f;


    private void Awake()
    {
        playerControl = new PlayerControls();
        CalculateStepsToNextEncounter();
    }
    private void OnEnable()
    {
        playerControl.Enable();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        partManager = GameObject.FindAnyObjectByType<PartyManager>();
       
        if(partManager.GetPosition() != Vector3.zero)      // if we have position save
        {
            transform.position = partManager.GetPosition();  // move player 
        }

    }
    void Update()
    {
        float x = playerControl.Player.Move.ReadValue<Vector2>().x;
        float z = playerControl.Player.Move.ReadValue<Vector2>().y;
        
        movement = new Vector3(x, 0, z).normalized;

        animator.SetBool(IS_WALK_PARAM, movement != Vector3.zero);


        if (x !=0  && x < 0)  
        {
            playerSprite.flipX = true;
        }
        if (x != 0 && x > 0) 
        {
            playerSprite.flipX = false;
        }


    }
    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);

        Collider[] colliders = Physics.OverlapSphere(transform.position,1,grassLayer);
        movingInGrass = colliders.Length != 0 && movement != Vector3.zero;

        if(movingInGrass) 
        {
            stepTimer += Time.fixedDeltaTime;
            if (stepTimer > TIME_PER_STEP)
            {
                stepsInGrass++;
                stepTimer = 0;
                
                if(stepsInGrass > stepToEncounter)
                {
                    //Check to see if we have reached an encounter
                    // -> chang the scene
                    stepsInGrass = 0;
                    partManager.SetPosition(transform.position);
                    SceneManager.LoadScene(BATTLE_SCENE);
                }
               

            }
            
        }

    }
    private void CalculateStepsToNextEncounter()
    {
        stepToEncounter = Random.Range(minStepToEncounter, maxStepToEncounter);
    }
}
