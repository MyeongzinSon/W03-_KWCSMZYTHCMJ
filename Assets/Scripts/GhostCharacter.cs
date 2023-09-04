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

    List<Collider2D> collidingDeadZones = new();

    void Interact()
    {
        Debug.Log($"Interaction has been called!");
    }

    public void OnInteractionCalled()
    {
        Interact();
        Debug.DrawRay(transform.position, Vector2.up, Color.red, 1);
    }

    protected override bool CheckDeadly(Collider2D collision)
    {
        if (collision.CompareTag("DeadZone"))
        {
            isDoneMoving = true;
            collidingDeadZones.Add(collision);
            StartCoroutine(CheckDeadlyCoroutine(collision));
            return true;
        }
        return false;
    }
    IEnumerator CheckDeadlyCoroutine(Collider2D collision, float delay = 0.05f)
    {
        yield return new WaitForSeconds(delay);
        if (collidingDeadZones.Contains(collision))
        {
            StartCoroutine(StageFailCoroutine());
            StageFail();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadZone"))
        {
            if (collidingDeadZones.Contains(collision))
            {
                collidingDeadZones.Remove(collision);
            }
            else
            {
                Debug.LogError($"����Ʈ�� ��ϵ��� ���� DeadZone��! {collision.gameObject.name}({collision.GetInstanceID()})");
            }
        }
    }
}

