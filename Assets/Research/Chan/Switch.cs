using System.Collections;
using System.Collections.Generic;
using Research.Chan;
using Unity.VisualScripting;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private Trap _trap;

    void Awake() {
        Init(_trap);
    }

    public void Init(Trap targetTrap) {
        _trap = targetTrap;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            _trap.PlayerToggleOnTrap();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            _trap.PlayerToggleOffTrap();
        }
    }
}
