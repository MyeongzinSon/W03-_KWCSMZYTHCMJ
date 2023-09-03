using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Research.Chan
{
    public class StageManager : MonoBehaviour
    {
        static StageManager _instance;
        public static StageManager Instance => _instance;
        
        public int stageCount = 4;              // 한 레벨에 존재하는 스테이지 수
        public GameObject stageLayout;          // 스테이지 생성 위치를 담고 있는 레이아웃
        private List<Vector3> _stagePositions = new List<Vector3>();  // 스테이지 생성 위치 리스트
        private int _currentStagePositionIndex = 0;
        
        public int currentLevelStageNumber;
        public List<Stage> stages;
        
        public List<bool> isStagesClear;
        private bool _isLevelClear;
        
        public List<bool> isStagesEnded;
        private bool _isEveryLevelEnded;
        
        private SpriteRenderer _filterSpriteRenderer;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            GetPositionsFromLayout();
            
            if (stages.Count != 0)
            {
                // 처음 스테이지는 1개 존재
                SetupStages();
            }
            else
            {
                Debug.LogWarning("No stages found in StageManager");
            }
        }
        
        /// <summary>
        /// 레이아웃으로 부터 스테이지 생성 위치 리스트 생성
        /// </summary>
        private void GetPositionsFromLayout()
        {
            if (stageLayout == null) Debug.LogWarning("No stageLayout found in StageManager");
            
            for (int i = 0; i < stageCount; i++)
            {
                Debug.Log(stageLayout.transform.GetChild(i).position);
                _stagePositions.Add(stageLayout.transform.GetChild(i).position);
                Debug.Log(_stagePositions[i]);
            }
        }
        
        private void SetupStages()
        {
            // 생성 위치 순환 방식
            SetupCurrentStage();
            // currentStageNumber에 따른 trap 설정
            SetupTrapsInCurrentStages();
        }
        
        /// <summary>
        /// 현재 레벨의 플레이어 스테이지 활성화 및 위치 지정
        /// </summary>
        private void SetupCurrentStage()
        {
            // 기존 스테이지 비활성화
            if (currentLevelStageNumber - stageCount >= 0)
            {
                stages[currentLevelStageNumber - stageCount].gameObject.SetActive(false);
            }
            stages[currentLevelStageNumber].gameObject.SetActive(true);
            stages[currentLevelStageNumber].transform.position = _stagePositions[_currentStagePositionIndex];
            _currentStagePositionIndex = (_currentStagePositionIndex + 1) % stageCount;
        }
        
        /// <summary>
        /// 현재 스테이지
        /// </summary>
        private void SetupTrapsInCurrentStages()
        {
            for (var i = currentLevelStageNumber; i > currentLevelStageNumber - 4; i--)
            {
                if (i < 0) continue;
                stages[i].SetupTraps(currentLevelStageNumber);
            }
        }

        public void StageClear(int stageNumber)
        {
            OnStageEnd(stageNumber);
        }
        
        public void StageFail(int stageNumber)
        {
            OnStageEnd(stageNumber);
        }
        
        private void OnStageEnd(int stageNumber)
        {
            isStagesEnded[stageNumber] = true;
            CheckLevelEnded();
            SetFilter(stageNumber);

            if (_isEveryLevelEnded)
            {
                CheckLevelClear();
            }
        }

        private void CheckLevelEnded()
        {
            for (var i = currentLevelStageNumber; i > currentLevelStageNumber - 4; i--)
            {
                if (i < 0) continue;
                if (isStagesEnded[i]) continue;
                
                _isEveryLevelEnded = false;
                return;
            }
            _isEveryLevelEnded = true;
        }
        
        /// <summary>
        /// 스테이지 클리어 여부에 따라 필터 색상 변경
        /// </summary>
        /// <param name="stageNumber"></param>
        private void SetFilter(int stageNumber)
        {
            stages[stageNumber].filter.SetActive(true);
            _filterSpriteRenderer = stages[stageNumber].filter.GetComponent<SpriteRenderer>();
            
            if (isStagesClear[stageNumber])
            {
                _filterSpriteRenderer.color = new Color(0, 1f, 0, .5f);
            }
            else
            {
                _filterSpriteRenderer.color = new Color(1f, 0, 0, .5f);
            }
        }
        
        /// <summary>
        /// 레벨 내 모든 스테이지 종료 시, 레벨 클리어 여부 확인 및 스테이지(트랩) 초기화
        /// </summary>
        private void CheckLevelClear()
        {
            for (var i = currentLevelStageNumber; i > currentLevelStageNumber - 4; i--)
            {
                if (i < 0) continue;
                if (!isStagesClear[i])
                {
                    _isLevelClear = false;
                    //TODO: ResetStages
                    GameManager.Instance.LevelFail();
                    return;
                }
                _isLevelClear = true;
            }

            if (_isLevelClear)
            {
                GameManager.Instance.LevelClear();
                currentLevelStageNumber++;
            }
        }
        
        public void OnStageRestart()
        {
            // 트랩, 필터 초기화
        }
        
        
        
        // 스테이지 생성 방식인 경우
        /*public void AddStage(Stage stage)
        {
            stages.Add(stage);
            stage.gameObject.SetActive(true);
            currentStageNumber++;
        }*/
    }
}