using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Serialization;

namespace Research.Chan
{
    public class Stage : MonoBehaviour
    {
        [Header("스테이지 벽 생성 필요 여부")]
        [SerializeField] public bool isWallNeeded = true;
        [Header("스테이지 벽 타일맵")]
        [SerializeField] public Tilemap wallTilemap;
        [Header("스테이지 벽 타일")]
        [SerializeField] public Tile wallTile;

        public int stageNumber;
        public GameObject startPoint;
        public GameObject endPoint;
        public List<Trap> traps;
        
        public GameObject filter;

        private StageData _stageData;


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
        
        public void Init(StageData stageData) {
            this._stageData = stageData;

            if (isWallNeeded)
            {
                SetupWalls();
            }
        }

        public void SetupTraps(int currentStageNumber)
        {
            foreach (var trap in traps)
            {
                trap.SetupTrap(currentStageNumber);
            }
        }

        private void SetupWalls() {
            if  (_stageData == null) {
                Debug.LogError("StageData is null");
                return;
            }

            List<Vector2Int> stageTilePosList = _stageData.StageTilePosList();
            foreach (var stageTilePos in stageTilePosList)
            {
                wallTilemap.SetTile(new Vector3Int(stageTilePos.x, stageTilePos.y, 0), wallTile);
            }
        }
    }
}