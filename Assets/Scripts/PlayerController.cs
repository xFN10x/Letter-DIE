using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{

    public Camera currentCamera;
    public GameObject Background;
    public int CameraZ;
    public bool CanJump;

    private PlayerInput plrInput;
    private Rigidbody2D rigid;
    private SpriteRenderer renderer;
    private InputAction MovementAction;
    private InputAction JumpAction;

    private float PlayerSpeedMultiplier = 15f;

    private float baseZ;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseZ = transform.position.z;

        plrInput = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        MovementAction = plrInput.actions.FindAction("Move");
        JumpAction = plrInput.actions.FindAction("Jump");
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
    }
    // Update is called once per frame
    void Update()
    {
        Background.transform.position = new Vector3(currentCamera.transform.position.x, 0, 5); //since the lines are going horizontal, it doesnt matter if it move right-left properly
    }
}
