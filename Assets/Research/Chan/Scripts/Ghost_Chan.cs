using System.Collections;
using System.Collections.Generic;
using Research.Chan;
using UnityEngine;

public class Ghost_Chan : GhostCharacter
{
    public int ghostStageNumber;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeadZone"))
        {
            StageManager.Instance.StageFail(ghostStageNumber);
        }
        else if (other.CompareTag("Goal"))
        {
            StageManager.Instance.StageClear(ghostStageNumber);
        }
    }
}
