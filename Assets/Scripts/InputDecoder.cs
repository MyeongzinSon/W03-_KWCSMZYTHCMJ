using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDecoder : MonoBehaviour
{
    Queue<InputInfo> inputQueue = null;
    IDecodeListener decodeListener;

    float decodeStartTime;
    bool isDecoding;

    public bool CanDecode { get { return inputQueue != null; } }

    private void Awake()
    {
        Initialize();
    }

    void Initialize()
    {

    }

    private void Update()
    {
        if (isDecoding && inputQueue.Count > 0)
        {
            var decodeTime = Time.time - decodeStartTime;
            CheckInputTime(decodeTime);
        }
    }

    void CheckInputTime(float decodeTime)
    {
        if (inputQueue.TryPeek(out var recordedInfo))
        {
            if (decodeTime >= recordedInfo.time)
            {
                inputQueue.Dequeue();
                decodeListener.SetDirection(recordedInfo.direction);
                if (recordedInfo.isInteractingThisFrame)
                {
                    decodeListener.OnInteractionCalled();
                }
                CheckInputTime(decodeTime);
            }
        }
        else
        {
            EndDecode();
        }
    }

    public void DecodeInputQueue(Queue<InputInfo> inputQueue)
    {
        // deep copy of inputQueue;
        this.inputQueue = new Queue<InputInfo>(inputQueue);
    }
    public void StartDecode(IDecodeListener decodeListener)
    {
        if (CanDecode)
        {
            if (inputQueue.Count == 0) { return; }
            if (isDecoding) { return; }

            this.decodeListener = decodeListener;
            isDecoding = true;
            decodeStartTime = Time.time;
        }
        else
        {
            Debug.LogError($"InputDecoder가 inputQueue를 가지고 있지 않음!");
        }
    }
    public void EndDecode()
    {
        if (isDecoding)
        {
            isDecoding = false;
        }
    }
}
