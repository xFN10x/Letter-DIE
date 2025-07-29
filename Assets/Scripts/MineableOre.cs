
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class MineableOre : InteractableObject
{
    //[HideInInspector]
    //public new Interactables InteractFunction;

    public Sprite State1;
    public Sprite State2;
    public Sprite State3;
    public Sprite State4;
    public Sprite State5;
    public int Health = 10;

    public AudioClip HitSound;
    public AudioClip BreakSound;

    private SpriteRenderer sprRenderer;
    private AudioSource source;
    private ParticleSystem particles;
    private void Start()
    {
        SuperStart();
        sprRenderer = GetComponent<SpriteRenderer>();
        source = GetComponent<AudioSource>();
        particles = GetComponent<ParticleSystem>();
    }

    protected override void Interacted(InputAction.CallbackContext obj)
    {
        if (beingTouched && CanTouch)
        {
            source.Stop();
            source.pitch = Random.Range(0.5f, 1f);
            source.PlayOneShot(HitSound);
            particles.Play();
            Health--;
            GameHandler.GiveGraphite(10);
            if (Health > 8)
            {
                //GameHandler.GiveGraphite(10);
                sprRenderer.sprite = State1;
            }
            else if (Health > 6)
            {
                //GameHandler.GiveGraphite(10);
                sprRenderer.sprite = State2;
            }
            else if (Health > 4)
            {
                //GameHandler.GiveGraphite(10);
                sprRenderer.sprite = State3;
            }
            else if (Health > 2)
            {
                //GameHandler.GiveGraphite(10);
                sprRenderer.sprite = State4;
            }
            else if (Health == 0)
            {
                GameHandler.GiveGraphite(40);
                sprRenderer.enabled = false;
                CanTouch = false;
                beingTouched = false;
                source.PlayOneShot(BreakSound);

            }
        }
    }
}
