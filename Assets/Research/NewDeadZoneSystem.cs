using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewDeadZoneSystem : MonoBehaviour
{
    //PlayerDetect GameObject
    private GameObject _playerDetect;
    //DeadZone GameObject
    private GameObject _deadZone;

    [SerializeField] private bool _isAuto;
    private bool _isCollidingWithPlayer;
    private NewPlayerSystem _targetPlayer;
    
    //Awake
    private void Awake() {
        //Find PlayerDetect GameObject
        _playerDetect = transform.Find("PlayerDetect").gameObject;
        //Find DeadZone GameObject
        _deadZone = transform.Find("DeadZone").gameObject;

        if (_isAuto) {
            _playerDetect.SetActive(true);
            _deadZone.SetActive(true);
        } else {
            _playerDetect.SetActive(false);
            _deadZone.SetActive(true);
        }
    }

    private void Update() {
        
    }
    
    //OnTriggerStay2D
    private void OnTriggerStay2D(Collider2D other)
    {
        NewPlayerSystem player = other.GetComponent<NewPlayerSystem>();
        //If other is Player Tag
        if (player != null) {
            if (_isAuto) {
                _playerDetect.SetActive(true);
                _deadZone.SetActive(false);
            }
            else {
                player.
            }
            _isCollidingWithPlayer = true;
            _targetPlayer = player;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            _isCollidingWithPlayer = false;
        }
    }
    
    //SetActive(true) Coroutine
    public IEnumerator SetActiveTrue() {
        //Wait for 1 second
        yield return new WaitForSeconds(2f);
        
        //Set PlayerDetect GameObject to Active
        _deadZone.SetActive(true);
    }
}
