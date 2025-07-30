using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class DogEnemy : IEnemy
{
    public enum State
    {
        look,
        bark,
        run,
        lunge,
        death,
    }

    public LayerMask GroundLayer;
    public Vector2 GroundCheckSize;
    public Vector2 GroundCheckPosFacingRight;
    public Vector2 GroundCheckPosFacingLeft;

    public Vector2 DeathCheckSize;
    public Vector2 DeathCheckPosFacingRight;
    //public Vector2 DeathCheckPosFacingLeft;

    private GameObject Player;

    public State currentState;
    public float moveAroundCooldown;
    public float moveAroundLength;
    public int moveAroundNextDir;

    public float BarkTransitionTimer;

    public float LungeRadius;
    public float StayAwayRadius;

    public AudioClip BarkSound;
    public AudioClip GrowlSound;
    public AudioClip DeathSound;

    public float LungeCooldown;

    private AudioSource source;
    public bool CanSwitchBackToRun;

    private float baseScaleX;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == PlayerLayer.value && animator.GetBool("AirBorne"))
        {
            playerController.HurtRandomArmLeg(true);
        }
    }
    //private Rigidbody2D rigid;
    protected override void PlayerSeen()
    {
        if (currentState == State.look)
        {
            currentState = State.bark;
            animator.SetTrigger("Angered");
            LungeCooldown = 3f;
        }
    }

    public void Bark()
    {
        source.PlayOneShot(BarkSound);
    }
    public void Growl()
    {
        source.PlayOneShot(GrowlSound);
    }


    public void Lunge()
    {
        if (!animator.GetBool("Airborne") && currentState == State.lunge)
        {
            animator.SetBool("Airborne", true);
            Vector2 direction = (Player.transform.position - transform.position) + (Vector3.up * 2);

            direction.Normalize();
            rigid.AddForce(direction * 100f, ForceMode2D.Impulse);
            print(rigid.linearVelocity);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SuperStart();
        Player = playerController.gameObject;
        moveAroundCooldown = Random.Range(2f, 6f);
        currentState = State.look;
        baseScaleX = transform.localScale.x;

        source = GetComponent<AudioSource>();
        playerController = Player.GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (currentState != State.death)
        {
            SuperFixedUpdate();

            Vector2 directionToPlayer = (Player.transform.position - transform.position);
            directionToPlayer.Normalize();

            if (Physics2D.OverlapBox((Vector2)transform.position + DeathCheckPosFacingRight, DeathCheckSize, 0f, PlayerLayer))
            {
                GetComponent<Collider2D>().enabled = false;
                GetComponent<Animator>().enabled = false;
                playerController.Jump(true, Vector2.up);
                animator.SetBool("Dead", true);
                Died();
            }


            if (Physics2D.OverlapBox((Vector2)transform.position + GetGroundCheckPos(), GroundCheckSize, 0f, GroundLayer))
            {
                animator.SetBool("Airborne", false);
                if (currentState == State.lunge && CanSwitchBackToRun)
                {
                    currentState = State.run;
                }
            }
            else
            {
                animator.SetBool("Airborne", true);

                print("not touchin ground");
                if (currentState == State.lunge && CanSwitchBackToRun == false)
                {
                    CanSwitchBackToRun = true;
                }
            }


            if (animator.GetFloat("XSpeed") != math.abs(rigid.linearVelocityX))
                animator.SetFloat("XSpeed", math.abs(rigid.linearVelocityX));

            if (Physics2D.OverlapCircle(transform.position, LungeRadius, PlayerLayer) && LungeCooldown <= 0)
            {
                currentState = State.lunge;
                CanSwitchBackToRun = false;
                animator.SetTrigger("Lunge");
                LungeCooldown = 3f;
            }


            if (currentState == State.run)
            {
                if (animator.GetBool("Walking") != true)
                    animator.SetBool("Walking", true);
                if (currentState != State.lunge)
                    if (Physics2D.OverlapCircle(transform.position, StayAwayRadius, PlayerLayer))
                    {
                        rigid.linearVelocityX = 10 * Mathf.Sign(-directionToPlayer.x);
                    }
                    else if (Physics2D.OverlapCircle(transform.position, StayAwayRadius + 3, PlayerLayer))
                        rigid.linearVelocityX = 0;
                    else
                        rigid.linearVelocityX = 10 * Mathf.Sign(directionToPlayer.x);
            }

            if (currentState == State.bark || currentState == State.run)
            {
                if (transform.localScale != new Vector3(baseScaleX * Mathf.Sign(directionToPlayer.x), transform.localScale.y, transform.localScale.z))
                    transform.localScale = new Vector3(baseScaleX * Mathf.Sign(directionToPlayer.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }
    [ExecuteInEditMode]

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.DrawWireCube(transform.position + (Vector3)DeathCheckPosFacingRight, DeathCheckSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, LungeRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, StayAwayRadius);
#endif

    }

    private Vector2 GetGroundCheckPos()
    {
        if (Single.IsNegative(transform.localScale.x))
        {
            return GroundCheckPosFacingLeft;
        }
        else return GroundCheckPosFacingRight;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.look:
                if (moveAroundCooldown <= 0)
                {
                    if (moveAroundLength <= 0)
                    {
                        //
                        moveAroundLength = Random.Range(1f, 3f);
                        moveAroundNextDir = Random.Range(-1, 1) == 0 ? 1 : -1;
                    }
                    else
                    {
                        if (animator.GetBool("Walking") != true)
                            animator.SetBool("Walking", true);
                        rigid.linearVelocityX = 5 * moveAroundNextDir;

                        if (transform.localScale != new Vector3(baseScaleX * moveAroundNextDir, transform.localScale.y, transform.localScale.z))
                            transform.localScale = new Vector3(baseScaleX * moveAroundNextDir, transform.localScale.y, transform.localScale.z);

                        moveAroundLength -= Time.deltaTime;
                        if (moveAroundLength <= 0)
                            moveAroundCooldown = Random.Range(2f, 6f);
                    }
                }

                if (moveAroundLength <= 0)
                {
                    if (animator.GetBool("Walking") != false)
                        animator.SetBool("Walking", false);
                    moveAroundCooldown -= Time.deltaTime;

                }

                break;
            case State.bark:
                if (BarkTransitionTimer <= 0)
                {
                    currentState = State.run;
                }
                BarkTransitionTimer -= Time.deltaTime;
                break;
            case State.run:
                LungeCooldown -= Time.deltaTime;
                break;
            case State.lunge:
                break;
            default:
                break;
        }
    }

    //private IEnumerator Die()
    //{

    //}

    protected override void Died()
    {
        currentState = State.death;
        GetComponent<ParticleSystem>().Play();
        source.PlayOneShot(DeathSound);
        gameHandler.GiveGraphite(100);
        gameHandler.PencilEraseEffect(transform.position, gameObject, gameObject.GetComponent<SpriteRenderer>());
    }
}
