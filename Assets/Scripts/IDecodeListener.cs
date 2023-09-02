using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDecodeListener
{
    public abstract Vector2 Direction { get; set; }
    public void SetDirection(Vector2 inputDirection)
    {
        Direction = inputDirection;
    }
    public abstract void OnInteractionCalled();
}
