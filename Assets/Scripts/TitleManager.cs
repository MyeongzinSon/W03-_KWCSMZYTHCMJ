using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour, PlayerInputActions.IPlayerActions
{
    PlayerInputActions inputs;
    void Awake()
    {
        inputs = new();
        inputs.Player.SetCallbacks(this);
        inputs.Enable();
    }

    public void StartStage()
    {
        inputs.Disable();
        var targetIndex = SceneManager.GetActiveScene().buildIndex + 1;
        Debug.Log(SceneManager.sceneCountInBuildSettings);
        SceneManager.LoadScene(targetIndex == SceneManager.sceneCountInBuildSettings ? 0 : targetIndex);
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartStage();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {

    }

    void PlayerInputActions.IPlayerActions.OnRestart(InputAction.CallbackContext context)
    {

    }

    void PlayerInputActions.IPlayerActions.OnPrevious(InputAction.CallbackContext context)
    {

    }
}
