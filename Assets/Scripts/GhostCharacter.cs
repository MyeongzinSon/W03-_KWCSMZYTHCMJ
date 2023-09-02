using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GhostCharacter : CharacterBase, IDecodeListener
{
    public Vector2 Direction 
    { 
        get => inputDirection; 
        set
        {
            inputDirection = value;
        }
    }

    private void Awake()
    {

    }

    void Update()
    {
        transform.Translate(3f * inputDirection * Time.deltaTime);
    }

    void Interact()
    {
        Debug.Log($"Interaction has been called!");
    }

    public void OnInteractionCalled()
    {
        Interact();
        Debug.DrawRay(transform.position, Vector2.up, Color.red, 1);
    }
}

