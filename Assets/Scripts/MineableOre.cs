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

    private SpriteRenderer sprRenderer;

    private void Start()
    {
        SuperStart();
        sprRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Interacted(InputAction.CallbackContext obj)
    {
        if (beingTouched)
        {
            Health--;
            if (Health > 8)
            {
                sprRenderer.sprite = State1;
            }
            else if (Health > 6)
            {
                sprRenderer.sprite = State2;
            }
            else if (Health > 4)
            {
                sprRenderer.sprite = State3;
            }
            else if (Health > 2)
            {
                sprRenderer.sprite = State2;
            }
        }
    }
}
