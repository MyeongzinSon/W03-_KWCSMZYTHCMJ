using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : CharacterBase, PlayerInputActions.IPlayerActions
{
    [SerializeField] InputRecorder recorder;
    [SerializeField] InputDecoder decoder;
    [SerializeField] GhostCharacter decodeTarget;

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

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartRecord();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            EndRecordAndStartDecode();
        }
    }

    void StartRecord()
    {
        recorder.StartRecord();
    }

    void EndRecordAndStartDecode()
    {
        recorder.EndRecord();

        recorder.TryGetInputQueue(out var recordedQueue);

        decoder.DecodeInputQueue(recordedQueue);
        decoder.StartDecode(decodeTarget);
    }

    void Interact()
    {
        Debug.Log($"Interaction has been called!");
    }
}
