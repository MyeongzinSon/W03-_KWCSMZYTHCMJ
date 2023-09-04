using System;
using System.Collections;
using System.Collections.Generic;
using Research.Chan;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Switch : MonoBehaviour
{
    [SerializeField] private List<TrapData> _trapDatas = new List<TrapData>();

    private int _switchStageNum;
    private int _curStageNum;

    private bool _isTurnedOn = false;

    void Awake() {
        //Init(_traps);
    }

    public void Init(int switchStageNum, int curStageNum) {
        _switchStageNum = switchStageNum;
        _curStageNum = curStageNum;
        foreach (var trap in _trapDatas) {
            if (trap.trap == null) print(name);
            trap.trap.InitFromSwitch(_switchStageNum);
        }
    }

    public void OnSwitchInteract() {
        foreach (var trapData in _trapDatas) {
            if (trapData.shouldTurnOn) {
                if (trapData.trap.IsToggledOn) {
                    trapData.trap.PlayerToggleOffTrap();
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<CharacterBase>() != null) {
            
        }
    }

    [Serializable]
    class TrapData {
        public SwitchableTrap trap;
        public bool shouldTurnOn = true;
    }
}
