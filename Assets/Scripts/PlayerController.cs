using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public PhysicsMaterial2D NormalPlayerMaterial;
    public PhysicsMaterial2D GrippingPlayerMaterial;
    public LayerMask GroundLayer;
    public Vector2 GroundCheckSize;
    public Vector2 GroundCheckPos;

    public Camera currentCamera;
    public GameObject Background;
    public Slider GripSlider;
    public int CameraZ;
    public bool CanJump;
    public float Grip = 1.0f;

    private PlayerInput plrInput;
    private Collider2D plrCollider;
    private Rigidbody2D rigid;
    private SpriteRenderer renderer;
    private InputAction MovementAction;
    private InputAction JumpAction;
    private InputAction GripAction;
    private ContactFilter2D GroundFilter;

    private float PlayerSpeedMultiplier = 15f;

    private float baseZ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GroundFilter.SetLayerMask(GroundLayer);

        baseZ = transform.position.z;

        plrInput = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        plrCollider = GetComponent<Collider2D>();
        MovementAction = plrInput.actions.FindAction("Move");
        JumpAction = plrInput.actions.FindAction("Jump");
        GripAction = plrInput.actions.FindAction("Grip");

        JumpAction.performed += jump;
        MovementAction.performed += checkDirection;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Jumpable"))
        {
            CanJump = true;
        }
    }

    private void checkDirection(InputAction.CallbackContext context)
    {
        Vector2 read = MovementAction.ReadValue<Vector2>();
        renderer.flipX = Single.IsNegative(read.x);
    }

    private void jump(InputAction.CallbackContext context)
    {
        if (!CanJump) return;
        CanJump = false;
        //if you can jump {
        Vector2 vel = rigid.linearVelocity;

        vel.y = 25;

        rigid.linearVelocity = vel;
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

    private void FixedUpdate()
    {
        //camera
        Vector2 lerped2 = Vector2.Lerp(currentCamera.transform.position, transform.position, 0.2f);
        currentCamera.transform.position = new Vector3(lerped2.x, lerped2.y, CameraZ);
        //player 
        Vector2 read = MovementAction.ReadValue<Vector2>();
        Vector2 vel = rigid.linearVelocity;

        vel.x = read.x * PlayerSpeedMultiplier;

        rigid.linearVelocity = vel;

        //jumping
        CanJump = Physics2D.OverlapBox(rigid.position + GroundCheckPos, GroundCheckSize, 0f, GroundLayer);
        //grip
        if (GripAction.IsPressed() && read.x != 0)
        {
            RaycastHit2D[] resualts = new RaycastHit2D[0];
            int hit = Physics2D.Raycast(transform.position, read.normalized, GroundFilter, resualts, 1f);
            if (hit >= 1)
            {
                rigid.sharedMaterial = GrippingPlayerMaterial;
            }
            else rigid.sharedMaterial = NormalPlayerMaterial;

        }
        else
        {
            rigid.sharedMaterial = NormalPlayerMaterial;
        }
    }
    // Update is called once per frame
    void Update()
    {
        Background.transform.position = new Vector3(currentCamera.transform.position.x, 0, 5); //since the lines are going horizontal, it doesnt matter if it move right-left properly
    }
}
