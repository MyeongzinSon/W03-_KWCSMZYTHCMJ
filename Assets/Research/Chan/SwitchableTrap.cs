using System;
using System.Collections;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;

namespace Research.Chan
{
    public class SwitchableTrap : Deadly
    {
        [SerializeField] protected int stageNumber;
        [SerializeField] protected int trapNumber;
        [SerializeField] protected bool isInvulnerable;
        [SerializeField] protected bool isTemporal;
        [SerializeField] private float toggleDuration = 3f;
        
        private GameObject _deadZone;
        private GameObject _playerDetect;
        
        private SpriteRenderer _spriteRenderer;
        
        private IEnumerator _coroutineTemporalTrap;
        
        private bool _isInitialized = false;
        private bool _isInitializedFromSwitch = false;
        private bool _hasTrapSet = false;
        private int _switchStageNum;
        private int _curStageNum;
        private bool _isToggledOn = true;
        public bool IsToggledOn => _isToggledOn;
        
        /*public Trap(int stageNumber, bool isInvulnerable, bool isTemporal)
        {
            this.stageNumber = stageNumber;
            this.isInvulnerable = isInvulnerable;
            this.isTemporal = isTemporal;
        }*/

        private void Awake()
        {
            //Init(1, 2);
        }

        private void Update() {
            if (_isInitialized && _isInitializedFromSwitch && !_hasTrapSet) {
                SetTrap();
                _hasTrapSet = true;
            }
        }

        public void Init(int trapStageNum, int curStageNum) {
            _isInitialized = true;
            _isToggledOn = true;

            _deadZone = transform.Find("DeadZone").gameObject;
            _playerDetect = transform.Find("PlayerDetect").gameObject;

            
            _spriteRenderer = transform.Find("DeadZone").GetComponent<SpriteRenderer>();
            //_spriteRenderer.color = new Color((trapNumber % 10) * 0.2f, .2f, .2f, 1f);
            
            _coroutineTemporalTrap = CoroutineTemporalTrap(toggleDuration);
            stageNumber = trapStageNum;
            _curStageNum = curStageNum;
        }

        public void InitFromSwitch(int switchStageNum) {
            _isInitializedFromSwitch = true;
            _switchStageNum = switchStageNum;
        }

        public void PlayerToggleOffTrap()
        {
            _deadZone.SetActive(false);
            _isToggledOn = false;
            StopCoroutine(_coroutineTemporalTrap);
            _coroutineTemporalTrap = CoroutineTemporalTrap(toggleDuration);
            StartCoroutine(_coroutineTemporalTrap);
        }

        public void PlayerToggleOnTrap() {
            if (isTemporal) {
                _isToggledOn = true;
                 _deadZone.SetActive(true);
            }
        }

        public void SetTrap()
        {
            if (_curStageNum >= _switchStageNum)
            {
                isInvulnerable = false;
            } else if (_curStageNum < _switchStageNum)
            {
                isInvulnerable = true;
            }
            
            if (isInvulnerable)
            {
                //플레이어 감지 활성화
                _playerDetect.SetActive(true);
            }
            else
            {
                //플레이어 감지 비활성화
                _playerDetect.SetActive(false);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isInvulnerable)
            {
                //비활성화
                if (other.CompareTag("Player"))
                {
                    PlayerToggleOffTrap();
                }
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (isInvulnerable)
            {
                if (isTemporal)
                {
                    if (other.CompareTag("Player"))
                    {
                        //재활성화
                        PlayerToggleOnTrap();
                    }
                }
            }
        }

        private IEnumerator CoroutineTemporalTrap(float duration)
        {
            yield return new WaitForSeconds(duration);
            PlayerToggleOnTrap();
        }
    }
}