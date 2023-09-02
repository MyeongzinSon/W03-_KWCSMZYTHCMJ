using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    //PlayerDetect GameObject
    private GameObject _playerDetect;
    //DeadZone GameObject
    private GameObject _deadZone;
    
    //Awake
    private void Awake()
    {
        //Find PlayerDetect GameObject
        _playerDetect = transform.Find("PlayerDetect").gameObject;
        //Find DeadZone GameObject
        _deadZone = transform.Find("DeadZone").gameObject;
    }
    
    //OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("AA Player Detected");
        //If other is Player Tag
        if (other.CompareTag("Player"))
        {
            //Player Detect Debug
            Debug.Log("Player Detected");
            //Set DeadZone GameObject to Inactive
            _deadZone.SetActive(false);
            //Start SetActiveTrue() Coroutine
            StartCoroutine(SetActiveTrue());
        }
    }
    
    //SetActive(true) Coroutine
    public IEnumerator SetActiveTrue()
    {
        //Wait for 1 second
        yield return new WaitForSeconds(2f);
        
        //Set PlayerDetect GameObject to Active
        _deadZone.SetActive(true);
    }
}
