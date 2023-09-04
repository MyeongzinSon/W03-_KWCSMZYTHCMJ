using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : CharacterBase, PlayerInputActions.IPlayerActions
{
    InputRecorder recorder;

    protected override void Awake()
    {
        base.Awake();

        PlayerInputActions inputs = new();
        inputs.Player.SetCallbacks(this);
        inputs.Enable();

        recorder = GetComponent<InputRecorder>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Interact();
            if (recorder.IsRecording)
            {
                recorder.RecordCurrentInput(true);
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>();
        if (recorder.IsRecording)
        {
            recorder.RecordCurrentInput(inputDirection);
        }
    }
    public void OnRestart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.Instance.StageFail();
        }
    }
    public void OnPrevious(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameManager.Instance.RestartPreviousStage();
            
        }
    }

    void Interact()
    {
        Debug.Log($"Interaction has been called!");
    }
}
