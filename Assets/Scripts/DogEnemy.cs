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
    }

    public GameObject Player;

    public State currentState;
    public float moveAroundCooldown;
    public float moveAroundLength;
    public int moveAroundNextDir;

    public float LungeRadius;

    public AudioClip BarkSound;
    public AudioClip GrowlSound;

    private AudioSource source;

    private float baseScaleX;
    //private Rigidbody2D rigid;
    protected override void PlayerSeen()
    {
        if (currentState == State.look)
        {
            currentState = State.bark;
            animator.SetTrigger("Angered");
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

    public void SwitchToRun()
    {
        currentState = State.run;
    }

    public void Lunge()
    {
        if (!animator.GetBool("Airborne"))
        {
            //animator.SetBool("Airborne", true);
            Vector2 direction = (Player.transform.position - transform.position);
            direction.Normalize();
            rigid.linearVelocity = direction * 100;
            print(direction);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SuperStart();
        moveAroundCooldown = Random.Range(2f, 6f);
        currentState = State.look;
        baseScaleX = transform.localScale.x;

        source = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        SuperFixedUpdate();

        //Debug.DrawRay(transform.position, rigid.linearVelocity, Color.red);
        //Debug.Log("Velocity: " + rigid.linearVelocity + " | Position: " + transform.position);

        if (animator.GetFloat("XSpeed") != math.abs(rigid.linearVelocityX))
            animator.SetFloat("XSpeed", math.abs(rigid.linearVelocityX));

        if (Physics2D.OverlapCircle(transform.position, LungeRadius, PlayerFilter.value))
        {
            animator.SetTrigger("Lunge");

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, LungeRadius);
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
            case State.run:
                break;
            case State.lunge:
                break;
            default:
                break;
        }
    }
}
