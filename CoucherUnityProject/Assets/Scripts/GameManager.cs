using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class GameManager : MonoBehaviour, IListener
{
    private Camera cam;
    private Vector3 originalCamPos;

    private int currentLevelIndex = 0;

    private int playerReachedExitCounter = 0;

    [SerializeField] private Tilemap[] levelTilemaps;
    [SerializeField] private LevelExit[] levelExits;

    public int CurrentRoomNumber => currentLevelIndex + 1;
    [Header("Camera movement")]
    [SerializeField] private float camGotoLevelAnimDuration;
    [SerializeField] private AnimationCurve camGotoLevelAnimCurve;

    [Header("Camera Shake")]
    [SerializeField] private Vector2 camShakeAxis;
    [SerializeField] private float camShakeDuration;
    [SerializeField] private AnimationCurve camShakeAnimCurve;

    [Header("Tiles")]
    [SerializeField] private Tile doorOpenTileTopWall;
    [SerializeField] private Tile doorClosedTileTopWall;

    private PlayerInput[] players;

    private Coroutine camShakeCoroutine;

    private TMPro.TextMeshProUGUI uIRoomText;

    private Image fadeInOutImage;
    [SerializeField] private float fadeInAnimDuration;
    [SerializeField] private AnimationCurve fadeInAnimCurve;
    [SerializeField] private AnimationCurve fadeOutAnimCurve;

    public static GameManager Instance;

    public TMP_Text controller1;
    public TMP_Text controller2;

    public PlayerInput[] PlayerArray;



    private void Awake()
    {
        PlayerArray[0].setPlayerNumber(1);
        PlayerArray[1].setPlayerNumber(2);
        Instance = this;
        cam = Camera.main;
        originalCamPos = cam.transform.position;
        players = FindObjectsOfType<PlayerInput>();
        fadeInOutImage = GameObject.Find("FadeInOutImage").GetComponent<Image>();
        uIRoomText = GameObject.Find("UIRoomText").GetComponent<TMPro.TextMeshProUGUI>();

    }

    private void OnEnable()
    {
        EventManager.AllWavesDone.AddListener(this, OnAllWavesDone);
        EventManager.PlayerEnteredExit.AddListener(this, OnPlayerEnteredExit);
        EventManager.GameOver.AddListener(this, OnGameOver);
        EventManager.Victory.AddListener(this, OnVictory);
        EventManager.EnemyDied.AddListener(this, OnEnemyDied);
    }

    private void OnDisable()
    {
        EventManager.AllWavesDone.RemoveListener(this);
        EventManager.PlayerEnteredExit.RemoveListener(this);
        EventManager.GameOver.RemoveListener(this);
        EventManager.Victory.RemoveListener(this);
        EventManager.EnemyDied.RemoveListener(this);
    }

    void Start()
    {
        StartCoroutine(InitRoutine());
    }

    void Update()
    {
        controller1.text = Input.GetJoystickNames()[0];
        controller2.text = Input.GetJoystickNames()[1];

        if (Input.GetKeyDown(KeyCode.F1))
        {
            StartNextLevel();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            EventManager.GameOver.RaiseEvent();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            EventManager.Victory.RaiseEvent();
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Sven");
        }
        Debug.LogWarning("CurrentRoomNumber: " + CurrentRoomNumber);
    }

    private void OnAllWavesDone()
    {
        StartCoroutine(EndofLevelRoutine());
    }

    private void OnPlayerEnteredExit()
    {
        playerReachedExitCounter++;
    }

    private void OnVictory()
    {
        StartCoroutine(VictoryRoutine());
    }

    private void OnGameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    private void OnEnemyDied(Enemy enemy)
    {
        if (camShakeCoroutine != null)
        {
            StopCoroutine(camShakeCoroutine);
            cam.transform.position = originalCamPos;
        }
        camShakeCoroutine = StartCoroutine(CamShakeRoutine());
    }

    private IEnumerator EndofLevelRoutine()
    {
        // Open door
        levelExits[currentLevelIndex - 1].OpenExit();
        SoundManager.instance.PlayAudioOnSource(SoundManager.instance.doorOpen, SoundManager.instance.audioSourceSFXUI, 0, 0);
        SoundManager.instance.PlayAudioOnSource(SoundManager.instance.levelClear, SoundManager.instance.audioSourceSFXUI, 0, 0);

        do
        {
            yield return null;
        } while (playerReachedExitCounter < 1);
        // Victory if no next level
        if (currentLevelIndex > levelTilemaps.Length - 1)
        {
            fadeInOutImage.color = Color.clear;
            fadeInOutImage.enabled = true;
            yield return StartCoroutine(Utility.LerpColorRoutine(fadeInOutImage, Color.black, 3f, false, fadeOutAnimCurve));
            EventManager.Victory.RaiseEvent();
            yield break;
        }
        StartNextLevel();
        playerReachedExitCounter = 0;
    }

    private IEnumerator InitRoutine()
    {
        // Fade into the first room
        fadeInOutImage.color = Color.black;
        yield return StartCoroutine(Utility.LerpColorRoutine(fadeInOutImage, Color.clear, fadeInAnimDuration, false, fadeInAnimCurve));
        StartNextLevel();
    }

    private void StartNextLevel()
    {
        StartCoroutine(StartNextLevelRoutine());
    }

    private IEnumerator StartNextLevelRoutine()
    {
        if (currentLevelIndex > levelTilemaps.Length - 1)
        {
            Debug.LogError("No more levels left to start.");
            yield break;
        }
        Vector3 trgPosCam = levelTilemaps[currentLevelIndex].gameObject.transform.position;
        trgPosCam.z = -10f;
        Vector3 trgPosPlayers = levelTilemaps[currentLevelIndex].gameObject.transform.position;
        // Move Players
        players[0].transform.position = trgPosPlayers + (Vector3.right * 2f);
        players[1].transform.position = trgPosPlayers + (Vector3.left * 2f);
        // Move camera to room
        if (cam.transform.position != trgPosCam)
            yield return StartCoroutine(Utility.MoveGameObjectRoutine(cam.transform, trgPosCam, camGotoLevelAnimDuration, camGotoLevelAnimCurve));

        originalCamPos = cam.transform.position;
        players[0]._playerCanMove = true;
        players[1]._playerCanMove = true;
        players[0].GetComponent<SpriteRenderer>().enabled = true;
        players[1].GetComponent<SpriteRenderer>().enabled = true;
        uIRoomText.text = "Room " + CurrentRoomNumber.ToString();
        Vector3 relativePosChange = Vector2.zero;
        if (currentLevelIndex > 0)
            relativePosChange = levelTilemaps[currentLevelIndex].transform.position - levelTilemaps[currentLevelIndex - 1].transform.position;
        EventManager.NewLevelStarted.RaiseEvent((currentLevelIndex, relativePosChange));
        currentLevelIndex++;
    }

    private IEnumerator GameOverRoutine()
    {
        fadeInOutImage.color = Color.clear;
        fadeInOutImage.enabled = true;
        yield return StartCoroutine(Utility.LerpColorRoutine(fadeInOutImage, Color.black, 2f, false, fadeOutAnimCurve));
        yield return new WaitForSeconds(1f);
        SoundManager.instance.PlayAudioOnSource(SoundManager.instance.bgMusicEnd, SoundManager.instance.audioSourceMain, 1, 5);
        UnityEngine.SceneManagement.SceneManager.LoadScene("EndScreenDefeat");
    }

    private IEnumerator VictoryRoutine()
    {
        fadeInOutImage.color = Color.clear;
        fadeInOutImage.enabled = true;
        yield return StartCoroutine(Utility.LerpColorRoutine(fadeInOutImage, Color.black, 2f, false, fadeOutAnimCurve));
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("EndScreenVictory");
    }

    private IEnumerator CamShakeRoutine()
    {
        float x = camShakeAxis.x;
        float y = camShakeAxis.y;
        Vector3 shakePos = originalCamPos + new Vector3(Random.Range(-x, x), Random.Range(-y, y), 0f);
        yield return StartCoroutine(Utility.MoveGameObjectRoutine(cam.transform, shakePos, camShakeDuration, camShakeAnimCurve));
        camShakeCoroutine = null;
    }
}
