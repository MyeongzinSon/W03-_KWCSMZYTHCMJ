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

        IsRecording = false;

        Debug.Log($"End Recording! : {Time.time}");
        Debug.Log($"Queue Count = {inputQueue.Count}");
        foreach (var input in inputQueue)
        {
            Debug.Log($"InputInfo = {input}");
        }
    }

    public void RecordCurrentInput(bool isInteractingThisFrame)
    {
        currentInputInfo.time = Time.time - recordStartTime;
        currentInputInfo.isInteractingThisFrame = isInteractingThisFrame;
        inputQueue.Enqueue(currentInputInfo);
    }

    public void RecordCurrentInput(Vector2 currentInput)
    {
        currentInputInfo.time = Time.time - recordStartTime;
        currentInputInfo.direction = currentInput;
        currentInputInfo.isInteractingThisFrame = false;
        inputQueue.Enqueue(currentInputInfo);
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
