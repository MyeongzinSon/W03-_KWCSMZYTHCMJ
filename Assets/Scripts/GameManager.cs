using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Research.Chan;
using DG.Tweening;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    const int k_maxStage = 4;

    static GameManager _instance;
    public static GameManager Instance => _instance;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject ghostPrefab;
    [SerializeField] Transform[] startingPoints = new Transform[k_maxStage];
    [SerializeField] StageResultIndicator[] stageResultIndicators;
    [SerializeField] SpriteRenderer[] stageBlinds;
    [SerializeField] bool isDebugMode = false;
    [SerializeField] float[] timeLimits;
    [SerializeField] GameObject timeLimitBar;
    [SerializeField] GameObject[] timeoutPanels;
    [SerializeField] GameObject levelClearUI;

    List<GameObject> playerAndGhosts = new();
    InputRecorder currentInputRecorder;
    Queue<InputInfo>[] inputQueues = new Queue<InputInfo>[k_maxStage - 1];
    List<Switch> switchs;
    List<SwitchableTrap> traps;
    Vector3 originalTimeLimitBarSize = new Vector3(21.35f, 0.45f, 1f);


    int currentStage = 1;
    public int clearedCount = 0;
    public int endedCount = 0;
    private bool isWaitForReloadScene = false;
    private bool canMoveNextScene = false;
    private PlayerCharacter currentPlayer;
    private IEnumerator timeLimiterCoroutine;

    private DG.Tweening.Core.TweenerCore<UnityEngine.Vector3, UnityEngine.Vector3, DG.Tweening.Plugins.Options.VectorOptions> timeLimitBarTween;

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
    }

    private void Update()
    {
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
        levelClearUI.SetActive(false);
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
                currentPlayer = player as PlayerCharacter;
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
        if (!isDebugMode) {
            for(i = 1; i <= k_maxStage; i++)
            {
                if (i <= currentStage) {
                    Color c = stageBlinds[i - 1].color;
                    //Debug.Log("cc " + c);
                    c.a = 0f;
                    stageBlinds[i - 1].color = c;
                } else {
                    Color c = stageBlinds[i - 1].color;
                    //Debug.Log("cc " + c);
                    c.a = 1f;
                    stageBlinds[i - 1].color = c;
                }
            }
        }
        
        StopAllCoroutines();
        isWaitForReloadScene = false;
        if (timeLimitBarTween != null) timeLimitBarTween.Kill();
        timeLimiterCoroutine = TimeLimiter();
        StartCoroutine(timeLimiterCoroutine);
    }

    public void LevelClear()
    {
        currentInputRecorder.EndRecord();
        currentInputRecorder.TryGetInputQueue(out var recordedQueue);
        inputQueues[currentStage - 1] = recordedQueue;
        currentStage++;
        if (!isDebugMode) {
            stageBlinds[currentStage - 1].DOFade(0f, 1f);
        }
        WaitForReloadScene();
    }

    public void StageFail()
    {
        endedCount++;
        currentInputRecorder.EndRecord();
    }

    public void StopTimeLimiter()
    {
        StopCoroutine(timeLimiterCoroutine);
        timeLimitBarTween.Kill();
    }

    public void RestartPreviousStage()
    {
        currentStage--;
        if (currentStage == 0) currentStage = 1;
        ReloadScene();
    }

    public void ClearReloadScene() {
        if (isWaitForReloadScene) {
            isWaitForReloadScene = false;
            ReloadScene();
        }
        if (canMoveNextScene)
        {
            GoToNextScene();
        }
    }

    IEnumerator TimeLimiter() {
        foreach (var panel in timeoutPanels) panel.SetActive(false);
        timeLimitBar.transform.localScale = originalTimeLimitBarSize;
        timeLimitBarTween = timeLimitBar.transform.DOScaleX(0f, timeLimits[currentStage - 1]).SetEase(Ease.Linear);
        yield return new WaitForSeconds(timeLimits[currentStage - 1]);
        clearedCount = 0;
        endedCount = 0;
        timeoutPanels[currentStage - 1].SetActive(true);
        currentPlayer.CannotMove();
    }

    void WaitForReloadScene() {
        isWaitForReloadScene = true;
    }

    public void ForceReloadScene() {
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
            if (currentStage == k_maxStage) StartCoroutine(ShowClearUICoroutine());
            else LevelClear();
        }
    }

    public void IndicateStageResult(int stageNum, bool isCleared)
    {
        stageResultIndicators[stageNum - 1].ShowResult(isCleared);
    }

    IEnumerator ShowClearUICoroutine()
    {
        levelClearUI.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        canMoveNextScene = true;
    }
    void GoToNextScene()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
