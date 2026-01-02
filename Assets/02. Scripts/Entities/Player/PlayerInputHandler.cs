using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput actions;

    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePosition { get; private set; }

    public bool DashPressed { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool AttackPressed { get; private set; }


    private void Awake()
    {
        actions = new PlayerInput();
    }

    private void OnEnable()
    {
        actions.Enable();
        actions.Player.WASD.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        actions.Player.WASD.canceled += ctx => MoveInput = Vector2.zero;

        actions.Player.MousePosition.performed += ctx =>
        {
            MousePosition = ctx.ReadValue<Vector2>();
        };

        actions.Player.Interact.performed += ctx => InteractPressed = true;
        actions.Player.Interact.canceled += ctx => InteractPressed = false;

        actions.Player.Dash.performed += ctx => DashPressed = true;
        actions.Player.Dash.canceled += ctx => DashPressed = false;

        actions.Player.Attack.performed += ctx => AttackPressed = true;
    }

    private void LateUpdate()
    {
        AttackPressed = false;
    }

    private void OnDisable()
    {
        actions.Player.Disable();
    }

}