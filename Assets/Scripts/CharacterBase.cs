using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    // value equals to Mathf.Sin(22.5 degree)
    const float k_inputCriterionSin = 0.38268343236f;
    [SerializeField] float speed;
    protected Rigidbody2D rigidBody;

    protected Vector2 inputDirection;

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        var velocity = inputDirection;
        velocity.x = Mathf.Abs(velocity.x) > k_inputCriterionSin ? Mathf.Sign(velocity.x) : 0;
        velocity.y = Mathf.Abs(velocity.y) > k_inputCriterionSin ? Mathf.Sign(velocity.y) : 0;
        rigidBody.velocity = velocity.normalized * speed;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EndingPoint")
        {
            GameManager.Instance.OneOfStagesCleared();
        }
    }
}
