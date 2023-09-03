using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Research.Chan
{
    public class Stage : MonoBehaviour
    {
        public int stageNumber;
        public GameObject startPoint;
        public GameObject endPoint;
        public List<Trap> traps;
        
        public GameObject filter;


        private void Awake()
        {
            startPoint = transform.Find("StartPoint").gameObject;
            endPoint = transform.Find("EndPoint").gameObject;
            
            filter = transform.Find("Filter").gameObject;

            if (traps.Count != 0)
            {
                foreach (var trap in traps)
                {
                    trap.SetTrapStageNumber(stageNumber);
                }
            }
            else
            {
                Debug.LogWarning("No traps found in stage " + stageNumber);
            }
        }
        
        public void SetupTraps(int currentStageNumber)
        {
            foreach (var trap in traps)
            {
                trap.SetupTrap(currentStageNumber);
            }
        }
    }
}