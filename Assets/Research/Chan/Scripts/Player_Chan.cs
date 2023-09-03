using System.Collections;
using System.Collections.Generic;
using Research.Chan;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Chan : PlayerCharacter
{
    public int playerStageNumber;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeadZone"))
        {
            StageManager.Instance.StageFail(playerStageNumber);
        }
        else if (other.CompareTag("Goal"))
        {
            StageManager.Instance.StageClear(playerStageNumber);
        }
    }
}
