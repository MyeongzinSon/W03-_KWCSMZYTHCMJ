using System;
using System.Collections;
using System.Collections.Generic;
using Research.Chan;
using Unity.VisualScripting;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] private List<TrapData> _traps = new List<TrapData>();

    private int _switchStageNum;
    private int _curStageNum;

    private bool _isTurnedOn = false;

    void Awake() {
        //Init(_traps);
    }

    public void Init(int switchStageNum, int curStageNum) {
        _switchStageNum = switchStageNum;
        _curStageNum = curStageNum;
        foreach (var trap in _traps) {
            trap.trap.InitFromSwitch(_switchStageNum);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            foreach (var trap in _traps) {
                if (trap.shouldTurnOn) {
                    if (trap.trap.IsToggledOn) {
                        trap.trap.PlayerToggleOffTrap();
                    }
                } else {
                    //trap.trap.PlayerToggleOnTrap();
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            foreach (var trap in _traps) {
                if (trap.shouldTurnOn) {
                    //trap.trap.PlayerToggleOnTrap();
                } else {
                    if (trap.trap.IsToggledOn) {
                        trap.trap.PlayerToggleOffTrap();
                    }
                }
            }
        }
    }

    [Serializable]
    class TrapData {
        public SwitchableTrap trap;
        public bool shouldTurnOn = true;
    }
}
