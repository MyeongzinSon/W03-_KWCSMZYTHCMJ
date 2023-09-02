using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : CharacterBase, PlayerInputActions.IPlayerActions
{
    InputRecorder recorder;

    private void Awake()
    {
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

    void Update()
    {
        transform.Translate(3f * inputDirection * Time.deltaTime);

    }

    void StartRecord()
    {
        recorder.StartRecord();
    }

    void EndRecordAndStartDecode()
    {
        recorder.EndRecord();

        recorder.TryGetInputQueue(out var recordedQueue);
    }

    void Interact()
    {
        Debug.Log($"Interaction has been called!");
    }
}
