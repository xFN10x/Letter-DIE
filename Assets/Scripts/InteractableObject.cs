using UnityEngine;
using UnityEngine.InputSystem;

public enum InputDevices
{
    Keyboard,
    DualSense,
    DualShock,
    Switch,
    Xbox
}

public enum Interactables
{
    Level
}

public enum InteractButton
{
    Main = 0,
    Secondary = 1
}

[RequireComponent(typeof(Collider2D))]
public class InteractableObject : MonoBehaviour
{
    public GameHandler GameHandler;
    public Interactables InteractFunction;
    public InteractButton InteractButton;
    public PlayerController Player;
    private PlayerInput PlayerInput;

    public GameObject StickyNoteObject;
    private ButtonGlyph StickyNoteGlyph;

    private GameObject StickyNoteButtonPrompt;
    protected bool beingTouched;
    public InputAction input;

    private void Start()
    {
        SuperStart();
    }

    protected void SuperStart()
    {
        input = new();

        StickyNoteButtonPrompt = GameObject.Instantiate(StickyNoteObject);
        StickyNoteButtonPrompt.transform.SetParent(gameObject.transform);
        StickyNoteButtonPrompt.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        Vector3 parentScale = transform.lossyScale;
        StickyNoteButtonPrompt.transform.localScale = new Vector3(
    parentScale.x != 0 ? 1f / parentScale.x : 1f,
    parentScale.y != 0 ? .5f / parentScale.y : .5f,
    1f
);
        StickyNoteGlyph = StickyNoteButtonPrompt.transform.GetChild(0).GetChild(1).GetComponent<ButtonGlyph>();
        StickyNoteGlyph.Player = Player.gameObject.GetComponent<PlayerInput>();
        StickyNoteGlyph.KeyboardImage = GetButtonSpriteByInteractButtonAndInputDevice(InputDevices.Keyboard, InteractButton);
        StickyNoteGlyph.PS4Image = GetButtonSpriteByInteractButtonAndInputDevice(InputDevices.DualShock, InteractButton);
        StickyNoteGlyph.PS5Image = GetButtonSpriteByInteractButtonAndInputDevice(InputDevices.DualSense, InteractButton);
        StickyNoteGlyph.SwitchImage = GetButtonSpriteByInteractButtonAndInputDevice(InputDevices.Switch, InteractButton);
        StickyNoteGlyph.XInputImage = GetButtonSpriteByInteractButtonAndInputDevice(InputDevices.Xbox, InteractButton);


        input.AddBinding(GetBindingByButton(false, InteractButton));
        input.AddBinding(GetBindingByButton(true, InteractButton));
        input.performed += Interacted;
        input.Enable();
    }

    protected virtual void Interacted(InputAction.CallbackContext obj)
    {
        print("Interacted!");
        if (beingTouched)
        {
            print("Interacted!");
            switch (InteractFunction)
            {
                case Interactables.Level:
                    GameHandler.CurrentMenu = Menu.Leveling;
                    break;
                default:
                    break;
            }
        }
    }

    public string GetBindingByButton(bool gamepad, InteractButton button)
    {
        switch (button)
        {
            default:
                if (!gamepad)

                    return "<Keyboard>/e";

                else return "<Gamepad>/buttonEast";
            case InteractButton.Secondary:
                if (!gamepad)

                    return "<Keyboard>/f";

                else return "<Gamepad>/buttonWest";
        }
    }
    public Sprite GetButtonSpriteByInteractButtonAndInputDevice(InputDevices device, InteractButton button)
    {
        return button switch
        {
            InteractButton.Secondary => device switch
            {
                InputDevices.Keyboard => Resources.Load<Sprite>("Buttons/Key&Mouse/T_F_Key_Dark"),
                InputDevices.DualSense => Resources.Load<Sprite>("Buttons/PS4/T_P4_Square_Color"),
                InputDevices.DualShock => Resources.Load<Sprite>("Buttons/PS5/T_P4_Square_Color"),
                InputDevices.Switch => Resources.Load<Sprite>("Buttons/Switch/T_S_Y"),
                InputDevices.Xbox => Resources.Load<Sprite>("Buttons/Xbox/T_X_X_Color"),
                _ => Resources.Load<Sprite>("Buttons/Xbox/T_X_X_Color"),
            },
            _ => device switch
            {
                InputDevices.Keyboard => Resources.Load<Sprite>("Buttons/Key&Mouse/T_E_Key_Dark"),
                InputDevices.DualSense => Resources.Load<Sprite>("Buttons/PS4/T_P4_Circle_Color"),
                InputDevices.DualShock => Resources.Load<Sprite>("Buttons/PS5/T_P5_Circle_Color"),
                InputDevices.Switch => Resources.Load<Sprite>("Buttons/Switch/T_S_A"),
                InputDevices.Xbox => Resources.Load<Sprite>("Buttons/Xbox/T_X_B_Color"),
                _ => Resources.Load<Sprite>("Buttons/Xbox/T_X_B_Color"),
            },
        };
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            beingTouched = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            beingTouched = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!beingTouched)
        {
            StickyNoteButtonPrompt.transform.localRotation = Quaternion.Euler(Vector3.Lerp(StickyNoteButtonPrompt.transform.localRotation.eulerAngles, new Vector3(90, 0, 0), 0.1f));
        }
        else
        {
            StickyNoteButtonPrompt.transform.localRotation = Quaternion.Euler(Vector3.Lerp(StickyNoteButtonPrompt.transform.localRotation.eulerAngles, new Vector3(0, 0, 0), 0.1f));
        }
    }
}
