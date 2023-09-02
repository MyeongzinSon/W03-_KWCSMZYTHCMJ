using System.Collections.Generic;
using UnityEngine;

namespace Research.Chan
{
    public class Stage : MonoBehaviour
    {
        public int stageNumber;
        public List<Trap> traps;


        private void Awake()
        {
            foreach (var trap in traps)
            {
                trap.SetTrapStageNumber(stageNumber);
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