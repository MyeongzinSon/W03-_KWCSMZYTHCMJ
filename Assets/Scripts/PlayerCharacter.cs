using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : CharacterBase, PlayerInputActions.IPlayerActions
{
    private void Awake()
    {
        PlayerInputActions inputs = new();
        inputs.Player.SetCallbacks(this);
        inputs.Enable();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Interact();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();
    }

    void Update()
    {
        Debug.Log($"Inputs = {inputDirection}");
    }

    void Interact()
    {
        Debug.Log($"Interaction has been called!");
    }
}
