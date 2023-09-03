using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public struct InputInfo
{
    public float time;
    public Vector2 direction;
    public bool isInteractingThisFrame;

    public InputInfo(float time, Vector2 direction, bool isInteractingThisFrame)
    {
        this.time = time;
        this.direction = direction;
        this.isInteractingThisFrame = isInteractingThisFrame;
    }
    public override string ToString()
    {
        return $"[{time} : {direction}, {isInteractingThisFrame}]";
    }

    public static InputInfo zero => new InputInfo(0, Vector2.zero, false);
}

public class InputRecorder : MonoBehaviour
{
    Queue<InputInfo> inputQueue;
    float recordStartTime = 0;
    public bool IsRecording { get; private set; }

    InputInfo currentInputInfo = InputInfo.zero;

    void Initialize()
    {
        IsRecording = false;
        recordStartTime = Time.time;
        inputQueue = new();
        currentInputInfo = InputInfo.zero;
    }

    public void StartRecord()
    {
        if (IsRecording) { return; }

        Initialize();
        IsRecording = true;
        Debug.Log($"Start Recording! : {Time.time}");
    }
    public void EndRecord()
    {
        if (!IsRecording) { return; }

        RecordCurrentInput(Vector2.zero, false, Time.deltaTime);
        IsRecording = false;

        Debug.Log($"End Recording! : {Time.time}");
        Debug.Log($"Queue Count = {inputQueue.Count}");
    }

    public void RecordCurrentInput(Vector2 currentInput, bool isInteractingThisFrame, float delay = 0)
    {
        currentInputInfo.time = Time.time - recordStartTime + delay;
        currentInputInfo.direction = currentInput;
        currentInputInfo.isInteractingThisFrame = isInteractingThisFrame;
        inputQueue.Enqueue(currentInputInfo);
    }
    public void RecordCurrentInput(bool isInteractingThisFrame)
    {
        RecordCurrentInput(currentInputInfo.direction, isInteractingThisFrame);
    }
    public void RecordCurrentInput(Vector2 currentInput)
    {
        RecordCurrentInput(currentInput, false);
    }

    public bool TryGetInputQueue(out Queue<InputInfo> returnedQueue)
    {
        if (inputQueue != null)
        {
            returnedQueue = inputQueue;
            inputQueue = null;
            return true;
        }
        else
        {
            returnedQueue = null;
            return false;
        }
    }
}
