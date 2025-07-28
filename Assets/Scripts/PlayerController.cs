using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public PhysicsMaterial2D NormalPlayerMaterial;
    public PhysicsMaterial2D GrippingPlayerMaterial;
    public LayerMask GroundLayer;
    public Vector2 GroundCheckSize;
    public Vector2 GroundCheckPos;

    public Camera currentCamera;
    public GameObject Background;
    //public GameObject Background2;
    public UnityEngine.UI.Slider GripSlider;
    public int CameraZ;
    public bool CanJump;
    public float Grip = 1.0f;

    private PlayerInput plrInput;
    private Collider2D plrCollider;
    private Animator plrAnimatior;
    private Rigidbody2D rigid;
    private SpriteRenderer sprRenderer;
    private InputAction MovementAction;
    private InputAction JumpAction;
    private InputAction GripAction;
    private CanvasGroup GripCG;
    private ContactFilter2D GroundFilter;
    private float InvertedControlTimer;
    private int InvertedTarget;

    private float desiriedGripAlpha;

    private float PlayerSpeedMultiplier = 15f;
    private Vector2 plrInputRead;
    private RaycastHit2D[] gripHitBuffer = new RaycastHit2D[1];
    private float baseZ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GroundFilter.SetLayerMask(GroundLayer);

        baseZ = transform.position.z;

        plrInput = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody2D>();
        sprRenderer = GetComponent<SpriteRenderer>();
        plrAnimatior = GetComponent<Animator>();
        plrCollider = GetComponent<Collider2D>();
        MovementAction = plrInput.actions.FindAction("Move");
        JumpAction = plrInput.actions.FindAction("Jump");
        GripAction = plrInput.actions.FindAction("Grip");
        GripCG = GripSlider.GetComponent<CanvasGroup>();

        JumpAction.performed += jump;
        //MovementAction.performed += CheckDirection;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Jumpable"))
        {
            CanJump = true;
        }
    }

    private void jump(bool IgnoreLawOfPhysics, Vector2 Direction)
    {
        if (!IgnoreLawOfPhysics)
            if (!CanJump) return;
        CanJump = false;
        //if you can jump {
        Vector2 vel = Direction * 25;

        rigid.linearVelocity = vel;
    }
    private void jump(InputAction.CallbackContext context)
    {
        jump(false, Vector2.up);
    }


    private void OnDrawGizmosSelected()
    {
        try
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(rigid.position + GroundCheckPos, GroundCheckSize);
        }
        catch { }
    }

    private void HandleCamera()
    {
        //camera

        Vector2 lerped2 = Vector2.Lerp(currentCamera.transform.position, transform.position, 0.2f);
        currentCamera.transform.position = new Vector3(lerped2.x, lerped2.y, CameraZ);
    }

    private void HandlePlayerInput()
    {
        bool isPlayerMoving = plrInputRead.x != 0;
        //player 

        if (plrAnimatior.GetBool("Walking") != isPlayerMoving)
            plrAnimatior.SetBool("Walking", isPlayerMoving);
        Vector2 vel = rigid.linearVelocity;
        if (isPlayerMoving)
        {
            bool isNeg = Single.IsNegative(plrInputRead.x);
            if (isNeg != sprRenderer.flipX)
                sprRenderer.flipX = isNeg;
        }

        vel.x = plrInputRead.x * PlayerSpeedMultiplier;

        rigid.linearVelocity = vel;

        //jumping
        CanJump = Physics2D.OverlapBox(rigid.position + GroundCheckPos, GroundCheckSize, 0f, GroundLayer);
    }

    private int getOppisitePlrDirection()
    {
        return (int)plrInputRead.x * -1;
    }

    private void HandleGrip()
    {
        //grip
        bool Gripping = false;
        if (GripAction.IsPressed() && plrInputRead.x != 0 && !CanJump)
        {
            int hit = Physics2D.Raycast(transform.position, plrInputRead.normalized, GroundFilter, gripHitBuffer, 1f);
            if (hit > 0 && Grip > 0)
            {

                Gripping = true;
                Grip -= Time.deltaTime;

                if (JumpAction.IsPressed())
                {
                    Gripping = false;
                    InvertedTarget = (int)math.round(getOppisitePlrDirection());
                    InvertedControlTimer = 0.3f;
                    jump(true, (Vector2.up + new Vector2(getOppisitePlrDirection(), 0).normalized));
                }
            }
        }

        desiriedGripAlpha = Gripping ? 1f : 0f;

        if (plrAnimatior.GetBool("Gripping") != Gripping)
            plrAnimatior.SetBool("Gripping", Gripping);

        if (rigid.sharedMaterial != (Gripping ? GrippingPlayerMaterial : NormalPlayerMaterial))
            rigid.sharedMaterial = Gripping ? GrippingPlayerMaterial : NormalPlayerMaterial;

    }

    private void FixedUpdate()
    {
        HandleCamera();
        HandlePlayerInput();
        HandleGrip();
    }
    // Update is called once per frame
    void Update()
    {
        plrInputRead = MovementAction.ReadValue<Vector2>();
        if (InvertedControlTimer > 0)
        {
            switch (InvertedTarget)
            {
                case 1: //right
                    plrInputRead = new float2(math.abs(plrInputRead.x), plrInputRead.y);
                    break;
                case -1:
                    plrInputRead = new float2(math.abs(plrInputRead.x) * -1f, plrInputRead.y);
                    break;
                default:
                    break;
            }
        }

        if (GripCG.alpha != desiriedGripAlpha)
            GripCG.alpha = Mathf.Lerp(GripCG.alpha, desiriedGripAlpha, 0.5f);

        if (GripSlider.value != math.clamp(Grip, 0.02f, 1))
            GripSlider.value = math.clamp(Grip, 0.02f, 1);

        InvertedControlTimer -= Time.deltaTime;
        if (InvertedControlTimer < 0) InvertedControlTimer = 0;

        if (plrAnimatior.GetBool("Airborne") != !CanJump)
            plrAnimatior.SetBool("Airborne", !CanJump);
        if (CanJump && Grip < 1f)
        {
            Grip += Time.deltaTime;
            Grip = Mathf.Clamp(Grip, 0f, 1f);
            desiriedGripAlpha = 0.3f;
        }

        Background.transform.position = new Vector3(currentCamera.transform.position.x, 0, 5); //since the lines are going horizontal, it doesnt matter if it move right-left properly
    }


}