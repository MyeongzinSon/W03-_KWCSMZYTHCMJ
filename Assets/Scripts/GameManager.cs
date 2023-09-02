using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    const int k_maxLevel = 4;

    static GameManager _instance;
    public static GameManager Instance => _instance;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject ghostPrefab;
    [SerializeField] Transform[] startingPoints = new Transform[k_maxLevel];

    InputRecorder currentInputRecorder;
    Queue<InputInfo>[] inputQueues = new Queue<InputInfo>[k_maxLevel - 1];


    int currentLevel = 1;

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
        if (Input.GetKeyDown(KeyCode.R))
        {
            StageFail();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            StageClear();
        }
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
        for (int i = 1; i <= currentLevel; i++)
        {
            var startingPoint = startingPoints[i - 1].position;
            if (i == currentLevel)
            {
                var playerGameObject = Instantiate(playerPrefab, startingPoint, Quaternion.identity);
                currentInputRecorder = playerGameObject.GetComponent<InputRecorder>();
                currentInputRecorder.StartRecord();
            }
            else
            {
                var ghostGameObject = Instantiate(ghostPrefab, startingPoint, Quaternion.identity);
                var decoder = ghostGameObject.GetComponent<InputDecoder>();
                var decodeTarget = ghostGameObject.GetComponent<GhostCharacter>();
                decoder.DecodeInputQueue(inputQueues[i - 1]);
                decoder.StartDecode(decodeTarget);
            }
        }
    }

    public void StageClear()
    {
        currentInputRecorder.EndRecord();
        currentInputRecorder.TryGetInputQueue(out var recordedQueue);
        inputQueues[currentLevel - 1] = recordedQueue;
        currentLevel++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void StageFail()
    {
        currentInputRecorder.EndRecord();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartStage();
    }

}
