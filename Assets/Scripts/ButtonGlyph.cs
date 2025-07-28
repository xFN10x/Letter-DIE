using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;

[RequireComponent(typeof(SpriteRenderer))]
public class ButtonGlyph : MonoBehaviour
{

    public PlayerInput Player;

    public Sprite KeyboardImage;
    public Sprite PS4Image;
    public Sprite SwitchImage;
    public Sprite XInputImage;
    public Sprite PS5Image;

    private SpriteRenderer sprRenderer;

    private void Start()
    {
        sprRenderer = GetComponent<SpriteRenderer>();
        ReSelectTexture(Player);
        Player.onControlsChanged += ReSelectTexture;
    }

    private void ReSelectTexture(PlayerInput input)
    {
        InputDevice CURRENT;
        if (Gamepad.current != null)
        {
            CURRENT = Gamepad.current;
        }
        else if (Keyboard.current != null)
        {
            CURRENT = Keyboard.current;
        }
        else

            return;


        print("Current Controller is " + CURRENT.name);

        sprRenderer.sprite = CURRENT switch
        {
            Keyboard => KeyboardImage,
            DualSenseGamepadHID => PS5Image,
            DualShockGamepad => PS4Image,
            SwitchProControllerHID => SwitchImage,
            XInputController => XInputImage,
            _ => XInputImage,
        };
    }



}
