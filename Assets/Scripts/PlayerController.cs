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
    public Vector2 GroundCheckPosFacingRight;
    public Vector2 GroundCheckPosFacingLeft;

    public Camera currentCamera;
    public GameObject Background;
    //public GameObject Background2;
    public UnityEngine.UI.Slider GripSlider;
    public int CameraZ;
    public bool CanJump;

    public float GripTimer;

    public float Grip = 1;
    public float Dexterity = 1;
    public float Strengh = 1;
    public float Endurance = 1;
    public float Robustness = 1;

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
#pragma warning disable IDE0044 // Add readonly modifier
    private RaycastHit2D[] gripHitBuffer = new RaycastHit2D[1];
#pragma warning restore IDE0044 // Add readonly modifier
    private float baseZ;
    private Vector3 baseScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GripTimer = Grip;

        GroundFilter.SetLayerMask(GroundLayer);

        baseScale = transform.localScale;
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

        JumpAction.performed += Jump;
        //MovementAction.performed += CheckDirection;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Jumpable"))
        {
            CanJump = true;
        }
    }

    private void Jump(bool IgnoreLawOfPhysics, Vector2 Direction)
    {
        //if (!plrAnimatior.GetBool("WallJump"))
        //    plrAnimatior.SetBool("WallJump", false);
        if (!IgnoreLawOfPhysics)
            if (!CanJump) return;
        CanJump = false;
        plrAnimatior.SetTrigger("Jump");
        //if you can jump {
        Vector2 vel = Direction.normalized * (24 + (Dexterity * 0.3f));

        rigid.linearVelocity = vel;
    }
    private void Jump(InputAction.CallbackContext context)
    {
        Jump(false, Vector2.up);
    }

    private Vector2 GetGroundCheckPos()
    {
        if (Single.IsNegative(transform.localScale.x))
        {
            return GroundCheckPosFacingLeft;
        }
        else return GroundCheckPosFacingRight;
    }

    private void OnDrawGizmosSelected()
    {
        try
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(rigid.position + GetGroundCheckPos(), GroundCheckSize);
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
        plrAnimatior.SetFloat("WalkXAbs", math.abs(plrInputRead.x * (Dexterity)));
        bool isPlayerMoving = plrInputRead.x != 0;
        if (plrAnimatior.GetBool("Walking") != isPlayerMoving)
            plrAnimatior.SetBool("Walking", isPlayerMoving);
        Vector2 vel = rigid.linearVelocity;
        if (isPlayerMoving)
        {
            bool isNeg = Single.IsNegative(plrInputRead.x);
            if (isNeg)
            {
                if (transform.localScale != new Vector3(-baseScale.x, baseScale.y, baseScale.z))
                {
                    transform.localScale = new Vector3(-baseScale.x, baseScale.y, baseScale.z);
                }
            }
            else if (transform.localScale != baseScale)
            {
                transform.localScale = baseScale;
            }
        }

        vel.x = plrInputRead.x * PlayerSpeedMultiplier;

        rigid.linearVelocity = vel;

        //jumping
        CanJump = Physics2D.OverlapBox(rigid.position + GetGroundCheckPos(), GroundCheckSize, 0f, GroundLayer);
    }

    private int GetOppisitePlrDirection()
    {
        return Math.Sign(plrInputRead.x * -1);
    }

    private void HandleGrip()
    {
        //grip
        bool Gripping = false;
        if (GripAction.IsPressed() && plrInputRead.x != 0 && !CanJump)
        {
            int hit = Physics2D.Raycast(transform.position, plrInputRead.normalized, GroundFilter, gripHitBuffer, 1f);
            if (hit > 0 && GripTimer > 0)
            {

                Gripping = true;
                GripTimer -= Time.deltaTime;

                if (JumpAction.IsPressed())
                {
                    //plrAnimatior.SetBool("WallJump", true);
                    Gripping = false;
                    InvertedTarget = Math.Sign(GetOppisitePlrDirection());
                    InvertedControlTimer = 0.3f;
                    Jump(true, (Vector2.up + new Vector2(GetOppisitePlrDirection(), 1).normalized));
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
        plrAnimatior.SetFloat("VelY", rigid.linearVelocityY);
        plrInputRead = MovementAction.ReadValue<Vector2>();
        if (InvertedControlTimer > 0)
        {
            switch (InvertedTarget)
            {
                case 1: //right
                    plrInputRead = new Vector2(MathF.Abs(plrInputRead.x), plrInputRead.y);
                    break;
                case -1:
                    plrInputRead = new Vector2(-MathF.Abs(plrInputRead.x), plrInputRead.y);
                    break;
                default:
                    Debug.LogWarning(("No inversion Target!"));
                    break;
            }
        }
        HandleCamera();
        HandlePlayerInput();
        HandleGrip();
    }
    // Update is called once per frame
    void Update()
    {
        if (PlayerSpeedMultiplier != 14 + (Dexterity * 0.3f))
            PlayerSpeedMultiplier = 14 + (Dexterity * 0.3f);

        if (GripCG.alpha != desiriedGripAlpha)
            GripCG.alpha = Mathf.Lerp(GripCG.alpha, desiriedGripAlpha, 0.5f);

        if (GripSlider.value != math.clamp(GripTimer, 0.02f, GripSlider.maxValue))
            GripSlider.value = math.clamp(GripTimer, 0.02f, GripSlider.maxValue);

        if (GripSlider.maxValue != (Grip / 4))
            GripSlider.maxValue = (Grip / 4);


        InvertedControlTimer -= Time.deltaTime;
        if (InvertedControlTimer < 0) InvertedControlTimer = 0;

        if (plrAnimatior.GetBool("Airborne") != !CanJump)
            plrAnimatior.SetBool("Airborne", !CanJump);
        if (CanJump && GripTimer < Grip)
        {
            GripTimer += Time.deltaTime * (Grip / 2);
            GripTimer = Mathf.Clamp(GripTimer, 0f, (Grip / 4));
            desiriedGripAlpha = 0.5f;
        }

        Background.transform.position = new Vector3(currentCamera.transform.position.x, 0, 5); //since the lines are going horizontal, it doesnt matter if it move right-left properly
    }


}