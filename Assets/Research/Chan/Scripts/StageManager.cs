using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Research.Chan
{
    public class StageManager : MonoBehaviour
    {
        // 싱글톤
        private static StageManager _instance;
        public static StageManager Instance => _instance;

        [Header("Stage Layout - 한 레벨의 스테이지 수")]
        [SerializeField] public StageLayout stageLayout;
        [Header("Stage Size - 스테이지 하나의 크기 (2의 배수 권장)")]
        [SerializeField] public Vector2Int stageLayoutSize;
        [Header("Stage Generation - 스테이지 생성 방식")]
        [SerializeField] public bool isStageGenerationAuto;
        [Header("Stage Prefab - 스테이지 수동 생성 방식")]
        [SerializeField] public List<GameObject> manualStagePrefabs;
        [Header("Stage Prefab - 스테이지 자동 생성 방식")]
        [SerializeField] public GameObject stagePrefab;
        
        [HideInInspector] public int currentLevelStageNumber;
        [Header("스테이지 클리어 현황")]
        public List<bool> isStagesClear;
        public List<bool> isStagesEnded;
        
        private int _stageMaxCount = 4;              // 한 레벨에 존재하는 스테이지 수
        private bool _isLevelClear;
        private bool _isEveryLevelEnded;
        private int _currentStagePositionIndex = 0;
        private Stage[] _instantiatedStages;
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
            
            //GetPositionsFromLayout();
            
            Init();
        }

        public void StageClear(int stageNumber)
        {
            OnStageEnd(stageNumber);
        }
        
        public void StageFail(int stageNumber)
        {
            OnStageEnd(stageNumber);
        }

        public void OnStageRestart()
        {
            // 트랩, 필터 초기화
        }
        
        /// <summary>
        /// 레이아웃으로 부터 스테이지 생성 위치 리스트 생성
        /// </summary>
        // private void GetPositionsFromLayout()
        // {
        //     for (int i = 0; i < stageMaxCount; i++)
        //     {
        //         //Debug.Log(stageLayout.transform.GetChild(i).position);
        //         _stagePositionsA.Add(stageLayout.transform.GetChild(i).position);
        //         //Debug.Log(_stagePositions[i]);
        //     }
        // }

        private void Init() {
            ApplyStageLayout();

            // 스테이지 이미 있는 경우 Destroy
            InitInstantiatedStageList();

            // 첫 번째 스테이지 생성
            currentLevelStageNumber = -1;
            SetNewLevel();
        }

        private void InitInstantiatedStageList() {
            foreach (Stage stage in _instantiatedStages) {
                if (stage != null) {
                    Destroy(stage.gameObject);
                }
            }
            _instantiatedStages = new Stage[_stageMaxCount];
        }

        private void SetNewLevel() {
            currentLevelStageNumber++;

            // 스테이지 레이아웃이 4개인 경우
            if (stageLayout == StageLayout.QUADRAPLE) {
                SetLevelQuadrant();
            } else {
                _currentStagePositionIndex = currentLevelStageNumber % _stageMaxCount;
            }

            if (_instantiatedStages[_currentStagePositionIndex] != null) {
                Destroy(_instantiatedStages[_currentStagePositionIndex].gameObject);
                _instantiatedStages[_currentStagePositionIndex] = null;
            }

            GameObject stageObj;
            Stage newStage;
            if (isStageGenerationAuto) {
                // 스테이지 자동 생성인 경우
                stageObj = Instantiate(stagePrefab, CalculateCurStagePos(), Quaternion.identity);
                
            } else {
                stageObj = Instantiate(manualStagePrefabs[currentLevelStageNumber], CalculateCurStagePos(), Quaternion.identity);
            }
            newStage = stageObj.GetComponent<Stage>();
            newStage.Init(GenerateStageData());
            _instantiatedStages[_currentStagePositionIndex] = newStage;

            isStagesClear = new List<bool>();
            isStagesEnded = new List<bool>();

            SetupTrapsInCurrentStages();
        }

        private void ApplyStageLayout() {
            switch (stageLayout) {
                case StageLayout.TRIPLE:
                    _stageMaxCount = 3;
                    break;
                case StageLayout.QUADRAPLE:
                    _stageMaxCount = 4;
                    break;
            }

            _instantiatedStages = new Stage[_stageMaxCount];
        }

        private void SetLevelQuadrant() {
            switch (currentLevelStageNumber % 4) {
                case 0:
                    _currentStagePositionIndex = 0;
                    break;
                case 1:
                    _currentStagePositionIndex = 1;
                    break;
                case 2:
                    _currentStagePositionIndex = 2;
                    break;
                case 3:
                    _currentStagePositionIndex = 3;
                    break;
            }
        }

        private StageData GenerateStageData() {
            StageData stageData = new StageData(
                currentLevelStageNumber, 
                _currentStagePositionIndex, 
                stageLayoutSize,
                stageLayout
            );

            return stageData;
        }

        private Vector2 CalculateCurStagePos() {
            Vector2 curStagePos = new Vector2(0, 0);

            switch (stageLayout) {
                case StageLayout.TRIPLE:
                    float startX = -(stageLayoutSize.x + 1);
                    curStagePos = new Vector2(
                        startX + (_currentStagePositionIndex * (stageLayoutSize.x + 1)),
                        0
                    );
                    break;
                case StageLayout.QUADRAPLE:
                    float x = (stageLayoutSize.x / 2) + 1;
                    float y = (stageLayoutSize.y / 2) + 1;

                    switch (_currentStagePositionIndex) {
                        case 0:
                            curStagePos = new Vector2(
                                -x,
                                y
                            );
                            break;
                        case 1:
                            curStagePos = new Vector2(
                                x,
                                y
                            );
                            break;
                        case 2:
                            curStagePos = new Vector2(
                                x,
                                -y
                            );
                            break;
                        case 3:
                            curStagePos = new Vector2(
                                -x,
                                -y
                            );
                            break;
                    }
                    break;
            }

            return curStagePos;
        }
        
        /// <summary>
        /// 현재 스테이지
        /// </summary>
        private void SetupTrapsInCurrentStages()
        {
            for (var i = 0; i < _stageMaxCount; i++)
            {
                _instantiatedStages[i].SetupTraps(currentLevelStageNumber);
            }
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
            _instantiatedStages[stageNumber].filter.SetActive(true);
            _filterSpriteRenderer = _instantiatedStages[stageNumber].filter.GetComponent<SpriteRenderer>();
            
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
        
        
        // 스테이지 생성 방식인 경우
        /*public void AddStage(Stage stage)
        {
            stages.Add(stage);
            stage.gameObject.SetActive(true);
            currentStageNumber++;
        }*/

        public enum StageLayout {
            TRIPLE,
            QUADRAPLE
        }
    }
}