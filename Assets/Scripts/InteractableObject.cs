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

    public Interactables InteractFunction;
    public InteractButton InteractButton;
    public PlayerController Player;

    public GameObject StickyNoteObject;
    private ButtonGlyph StickyNoteGlyph;

    private GameObject StickyNoteButtonPrompt;
    private bool beingTouched;

    void Start()
    {
        StickyNoteButtonPrompt = GameObject.Instantiate(StickyNoteObject);
        StickyNoteButtonPrompt.transform.SetParent(gameObject.transform);
        StickyNoteButtonPrompt.transform.SetLocalPositionAndRotation(new Vector3(-4, 7, 0), Quaternion.Euler(0, 0, 0));
        StickyNoteGlyph = StickyNoteButtonPrompt.transform.GetChild(0).GetChild(1).GetComponent<ButtonGlyph>();
        StickyNoteGlyph.Player = Player.gameObject.GetComponent<PlayerInput>();
        StickyNoteGlyph.KeyboardImage = GetButtonSpriteByInteractButtonAndInputDevice(InputDevices.Keyboard, InteractButton);
        StickyNoteGlyph.PS4Image = GetButtonSpriteByInteractButtonAndInputDevice(InputDevices.DualShock, InteractButton);
        StickyNoteGlyph.PS5Image = GetButtonSpriteByInteractButtonAndInputDevice(InputDevices.DualSense, InteractButton);
        StickyNoteGlyph.SwitchImage = GetButtonSpriteByInteractButtonAndInputDevice(InputDevices.Switch, InteractButton);
        StickyNoteGlyph.XInputImage = GetButtonSpriteByInteractButtonAndInputDevice(InputDevices.Xbox, InteractButton);
    }

    public Sprite GetButtonSpriteByInteractButtonAndInputDevice(InputDevices device, InteractButton button)
    {
        return button switch
        {
            InteractButton.Secondary => device switch
            {
                InputDevices.Keyboard => Resources.Load<Sprite>("Buttons/Key&Mouse/T_E_Key_Dark"),
                InputDevices.DualSense => Resources.Load<Sprite>("Buttons/Key&Mouse/T_E_Key_Dark"),
                InputDevices.DualShock => Resources.Load<Sprite>("Buttons/Key&Mouse/T_E_Key_Dark"),
                InputDevices.Switch => Resources.Load<Sprite>("Buttons/Key&Mouse/T_E_Key_Dark"),
                InputDevices.Xbox => Resources.Load<Sprite>("Buttons/Key&Mouse/T_E_Key_Dark"),
                _ => Resources.Load<Sprite>("Buttons/Key&Mouse/T_E_Key_Dark"),
            },
            _ => device switch
            {
                InputDevices.Keyboard => Resources.Load<Sprite>("Buttons/Key&Mouse/T_E_Key_Dark"),
                InputDevices.DualSense => Resources.Load<Sprite>("Buttons/Key&Mouse/T_E_Key_Dark"),
                InputDevices.DualShock => Resources.Load<Sprite>("Buttons/Key&Mouse/T_E_Key_Dark"),
                InputDevices.Switch => Resources.Load<Sprite>("Buttons/Key&Mouse/T_E_Key_Dark"),
                InputDevices.Xbox => Resources.Load<Sprite>("Buttons/Key&Mouse/T_E_Key_Dark"),
                _ => Resources.Load<Sprite>("Buttons/Key&Mouse/T_E_Key_Dark"),
            },
        };
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            beingTouched = true;
        }
        else
        {
            beingTouched = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!beingTouched)
        {
            StickyNoteButtonPrompt.transform.localRotation = Quaternion.Euler(Vector3.Lerp(StickyNoteButtonPrompt.transform.localRotation.eulerAngles, new Vector3(90, 0, 0), 0.5f));
        }
        else
        {
            StickyNoteButtonPrompt.transform.localRotation = Quaternion.Euler(Vector3.Lerp(StickyNoteButtonPrompt.transform.localRotation.eulerAngles, new Vector3(0, 0, 0), 0.5f));
        }
    }
}
