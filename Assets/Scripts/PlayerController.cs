using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Limb : int
{
    Head,
    Torso,
    LeftArm, RightArm,
    LeftLeg, RightLeg,
}

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    public GameHandler GameHandler;

    public PhysicsMaterial2D NormalPlayerMaterial;
    public PhysicsMaterial2D GrippingPlayerMaterial;
    public LayerMask GroundLayer;
    public Vector2 GroundCheckSize;
    public Vector2 GroundCheckPosFacingRight;
    public Vector2 GroundCheckPosFacingLeft;

    public AudioClip JumpSound;
    public AudioClip WallJumpExtraSound;
    public AudioClip HurtSound;

    public AudioClip[] StepSounds;

    public GameObject RightArmRoot;
    public GameObject LeftArmRoot;
    public GameObject RightLegRoot;
    public GameObject LeftLegRoot;

    public GameObject RightArmIK;
    public GameObject LeftArmIK;
    public GameObject RightLegIK;
    public GameObject LeftLegIK;

    public int RightArmHealth;
    public int LeftArmHealth;
    public int RightLegHealth;
    public int LeftLegHealth;

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
    private AudioSource source;
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
        source = GetComponent<AudioSource>();
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

    public void Jump(bool IgnoreLawOfPhysics, Vector2 Direction)
    {
        if (!IgnoreLawOfPhysics)
            if (!CanJump) return;
        CanJump = false;
        plrAnimatior.SetTrigger("Jump");
        //if you can jump {
        Vector2 vel = Direction.normalized * (24 + (Dexterity * 0.3f));

        rigid.linearVelocity = vel;
        source.PlayOneShot(JumpSound);

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

    public void StepSound()
    {
        int randomPick = UnityEngine.Random.Range(0, StepSounds.Length - 1);

        source.PlayOneShot(StepSounds[randomPick]);
    }

    public void HurtRandomArmLeg(bool AttackOtherAfterAllGone)
    {
        int random = UnityEngine.Random.Range(2, 5);

        HurtLimb(random);
    }

    public void HurtLimb(int limb)
    {
        source.PlayOneShot(HurtSound);
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
                    GripTimer -= 0.2f;
                    source.PlayOneShot(WallJumpExtraSound);

                    Jump(true, (Vector2.up + new Vector2(GetOppisitePlrDirection(), 1).normalized));
                }
            }
        }

        desiriedGripAlpha = !CanJump && GripTimer <= 0.75f + Grip ? 1f : (CanJump && GripTimer < 0.75f + Grip ? 0.5f : 0);

        if (plrAnimatior.GetBool("Gripping") != Gripping)
            plrAnimatior.SetBool("Gripping", Gripping);

        if (rigid.sharedMaterial != (Gripping ? GrippingPlayerMaterial : NormalPlayerMaterial))
            rigid.sharedMaterial = Gripping ? GrippingPlayerMaterial : NormalPlayerMaterial;

    }

    private void FixedUpdate()
    {

        if (plrAnimatior.GetFloat("VelY") != rigid.linearVelocityY)
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

        if (GripSlider.maxValue != 0.75f + (Grip / 4))
            GripSlider.maxValue = 0.75f + (Grip / 4);

        if (CanJump && GripTimer < 0.75f + Grip)
        {
            GripTimer += Time.deltaTime * (Grip / 2);
            GripTimer = Mathf.Clamp(GripTimer, 0f, 0.75f + (Grip / 4));
        }

        InvertedControlTimer -= Time.deltaTime;
        if (InvertedControlTimer < 0) InvertedControlTimer = 0;

        if (plrAnimatior.GetBool("Airborne") != !CanJump)
            plrAnimatior.SetBool("Airborne", !CanJump);


        Background.transform.position = new Vector3(currentCamera.transform.position.x, 0, 5); //since the lines are going horizontal, it doesnt matter if it move right-left properly
    }


}