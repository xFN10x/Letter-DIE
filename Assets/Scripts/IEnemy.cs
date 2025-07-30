using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public abstract class IEnemy : MonoBehaviour
{
    public GameHandler gameHandler;
    public PlayerController playerController;

    public float AngerRadius;
    protected Rigidbody2D rigid;
    protected Collider2D collide;
    protected Animator animator;
    public LayerMask PlayerLayer;

    protected void SuperStart()
    {
        rigid = this.GetComponent<Rigidbody2D>();
        collide = this.GetComponent<Collider2D>();
        animator = this.GetComponent<Animator>();
    }

    [ExecuteInEditMode]
    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gameObject.transform.position, AngerRadius);
#endif
    }

    protected abstract void PlayerSeen();
    protected abstract void Died();

    protected void SuperFixedUpdate()
    {
        if (Physics2D.OverlapCircle(gameObject.transform.position, AngerRadius, PlayerLayer.value))
        {
            this.PlayerSeen();
        }
    }
}
