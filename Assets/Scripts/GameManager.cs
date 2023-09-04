using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Research.Chan;
using Unity.VisualScripting;
//using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    const int k_maxStage = 4;

    static GameManager _instance;
    public static GameManager Instance => _instance;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject ghostPrefab;
    [SerializeField] Transform[] startingPoints = new Transform[k_maxStage];
    [SerializeField] StageResultIndicator[] stageResultIndicators;

    List<GameObject> playerAndGhosts = new();
    InputRecorder currentInputRecorder;
    Queue<InputInfo>[] inputQueues = new Queue<InputInfo>[k_maxStage - 1];
    List<Switch> switchs;
    List<SwitchableTrap> traps;

    bool isWaitForReloadScene = false;
    //PlayerInputActions inputAction;


    int currentStage = 1;
    public int clearedCount = 0;
    public int endedCount = 0;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            // inputAction = new PlayerInputActions();
            // inputAction.Player.Interact.started += ctx => {
            //     if (isWaitForReloadScene) ReloadScene();
            // };
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ForceReloadScene();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            RestartPreviousStage();
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            LevelClear();
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            IndicateStageResult(1, true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            IndicateStageResult(2, true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            IndicateStageResult(3, true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            IndicateStageResult(4, true);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            IndicateStageResult(1, false);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            IndicateStageResult(2, false);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            IndicateStageResult(3, false);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            IndicateStageResult(4, false);
        }
#endif
    }


    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartStage();
    }
    private void Initialize()
    {
    }

    public void StartStage()
    {
        switchs = new List<Switch>();
        for (int si = 0; si < k_maxStage; si++) {
            var l = GameObject.Find("Stage " + (si + 1));
            List<Switch> sl = l.GetComponentsInChildren<Switch>().ToList();
            if (sl.Count == 0) continue;
            sl.ForEach(s => s.Init(si + 1, currentStage));
            switchs.AddRange(sl);
        }

        traps = new List<SwitchableTrap>();
        for (int ti = 0; ti < k_maxStage; ti++) {
            var l = GameObject.Find("Stage " + (ti + 1));
            List<SwitchableTrap> tl = l.GetComponentsInChildren<SwitchableTrap>().ToList();
            if (tl.Count == 0) continue;
            tl.ForEach(t => t.Init(ti + 1, currentStage));
            traps.AddRange(tl);
        }
        
        playerAndGhosts.Clear();
        stageResultIndicators.ToList().ForEach(i => i.ClearResult());

        int i = 1;
        for (; i <= currentStage; i++)
        {
            var startingPoint = startingPoints[i - 1].position;
            if (i == currentStage)
            {
                var playerGameObject = Instantiate(playerPrefab, startingPoint, Quaternion.identity);
                currentInputRecorder = playerGameObject.GetComponent<InputRecorder>();
                currentInputRecorder.StartRecord();
                var player = playerGameObject.GetComponent<CharacterBase>();
                player.SetStageNum(i);
            }
            else
            {
                var ghostGameObject = Instantiate(ghostPrefab, startingPoint, Quaternion.identity);
                var decoder = ghostGameObject.GetComponent<InputDecoder>();
                var decodeTarget = ghostGameObject.GetComponent<GhostCharacter>();
                decoder.DecodeInputQueue(inputQueues[i - 1]);
                decoder.StartDecode(decodeTarget);
                var ghost = ghostGameObject.GetComponent<CharacterBase>();
                ghost.SetStageNum(i);
            }
        }
        for(; i <= k_maxStage; i++)
        {

        }

        isWaitForReloadScene = false;
    }

    public void LevelClear()
    {
        currentInputRecorder.EndRecord();
        currentInputRecorder.TryGetInputQueue(out var recordedQueue);
        inputQueues[currentStage - 1] = recordedQueue;
        currentStage++;
        //ReloadScene();
        WaitForReloadScene();
    }
    public void StageFail()
    {
        endedCount++;
        // currentInputRecorder.EndRecord();
        // ReloadScene();
    }
    public void RestartPreviousStage()
    {
        currentStage--;
        if (currentStage == 0) currentStage = 1;
        ReloadScene();
    }

    void WaitForReloadScene() {
        isWaitForReloadScene = true;
    }

    public void ClearReloadScene() {
        if (isWaitForReloadScene) ReloadScene();
    }

    void ForceReloadScene() {
        clearedCount = 0;
        endedCount = 0;
        Debug.Log("cc B " + clearedCount);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ReloadScene()
    {
        clearedCount = 0;
        endedCount = 0;
        Debug.Log("cc B " + clearedCount);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartStage();
    }

    public void OneOfStagesCleared()
    {
        endedCount++;
        clearedCount++;
        Debug.Log("cc A " + clearedCount);

        if (clearedCount >= currentStage)
        {
            Debug.Log(clearedCount);
            if (currentStage == k_maxStage) Debug.Log("Level Cleared!");
            else LevelClear();
        }
    }

    public void IndicateStageResult(int stageNum, bool isCleared)
    {
        stageResultIndicators[stageNum - 1].ShowResult(isCleared);
    }
}
