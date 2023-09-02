using System;
using System.Collections;
using UnityEngine;

namespace Research.Chan
{
    public class Trap : MonoBehaviour
    {
        [SerializeField] protected int stageNumber;
        [SerializeField] protected int trapNumber;
        [SerializeField] protected bool isInvulnerable;
        [SerializeField] protected bool isTemporal;
        
        private GameObject _deadZone;
        private GameObject _playerDetect;
        private GameObject _visualIsTemporal;
        
        private SpriteRenderer _spriteRenderer;
        
        /*public Trap(int stageNumber, bool isInvulnerable, bool isTemporal)
        {
            this.stageNumber = stageNumber;
            this.isInvulnerable = isInvulnerable;
            this.isTemporal = isTemporal;
        }*/

        private void Awake()
        {
            _deadZone = transform.Find("DeadZone").gameObject;
            _playerDetect = transform.Find("PlayerDetect").gameObject;
            _visualIsTemporal = transform.Find("VisualIsTemporal").gameObject;

            _visualIsTemporal.gameObject.SetActive(isTemporal);
            
            _spriteRenderer = transform.Find("DeadZone").GetComponent<SpriteRenderer>();
            _spriteRenderer.color = new Color((trapNumber % 10) * 0.2f, .2f, .2f, 1f);
        }

        public void SetTrap(int curStageNumber)
        {
            if (stageNumber < curStageNumber)
            {
                isInvulnerable = false;
            }
            
            if (isInvulnerable)
            {
                //플레이어 감지 활성화
                _deadZone.SetActive(false);
                _playerDetect.SetActive(true);
            }
            else
            {
                //플레이어 감지 비활성화
                _deadZone.SetActive(true);
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
                    _deadZone.SetActive(false);
                    _visualIsTemporal.SetActive(false);
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
                        _deadZone.SetActive(true);
                        _visualIsTemporal.SetActive(true);
                    }
                }
            }
        }
    }
}