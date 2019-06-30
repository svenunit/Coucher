using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour, IListener
{
    private Camera cam;
    private Vector3 originalCamPos;

    private int currentLevelIndex = 0;

    private int playerReachedExitCounter = 0;

    [SerializeField] private Tilemap[] levelTilemaps;
    [SerializeField] private LevelExit[] levelExits;
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

    private Image fadeInOutImage;
    [SerializeField] private float fadeInAnimDuration;
    [SerializeField] private AnimationCurve fadeInAnimCurve;

    private void Awake()
    {
        cam = Camera.main;
        originalCamPos = cam.transform.position;
        players = FindObjectsOfType<PlayerInput>();
        fadeInOutImage = GameObject.Find("FadeInOutImage").GetComponent<Image>();
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
        if (Input.GetKeyDown(KeyCode.F1))
        {
            StartNextLevel();
            // StartCoroutine(EndofLevelRoutine());
        }
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
            StopCoroutine(camShakeCoroutine);
        camShakeCoroutine = StartCoroutine(CamShakeRoutine());
    }

    private IEnumerator EndofLevelRoutine()
    {
        // Victory if no next level
        if (currentLevelIndex > levelTilemaps.Length - 1)
        {
            EventManager.Victory.RaiseEvent();
            yield break;
        }
        // Open door
        levelExits[currentLevelIndex - 1].OpenExit();
        SoundManager.instance.PlayAudioOnSource(SoundManager.instance.doorOpen, SoundManager.instance.audioSourceSFXUI, 0, 0);
        SoundManager.instance.PlayAudioOnSource(SoundManager.instance.levelClear, SoundManager.instance.audioSourceSFXUI, 0, 0);



        do
        {
            yield return null;
        } while (playerReachedExitCounter < 1);

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
        Vector3 trgPosCam = levelTilemaps[currentLevelIndex].gameObject.transform.position;
        trgPosCam.z = -10f;
        Vector3 trgPosPlayers = levelTilemaps[currentLevelIndex].gameObject.transform.position;
        // Move Players
        players[0].transform.position = trgPosPlayers + (Vector3.right * 2f);
        players[1].transform.position = trgPosPlayers + (Vector3.left * 2f);
        // Move camera to room
        if (cam.transform.position != trgPosCam)
            yield return StartCoroutine(Utility.MoveGameObjectRoutine(cam.transform, trgPosCam, camGotoLevelAnimDuration, camGotoLevelAnimCurve));

        players[0]._playerCanMove = true;
        players[1]._playerCanMove = true;
        players[0].GetComponent<SpriteRenderer>().enabled = true;
        players[1].GetComponent<SpriteRenderer>().enabled = true;
        Vector3 relativePosChange = Vector2.zero;
        if (currentLevelIndex > 0)
            relativePosChange = levelTilemaps[currentLevelIndex].transform.position - levelTilemaps[currentLevelIndex - 1].transform.position;
        EventManager.NewLevelStarted.RaiseEvent((currentLevelIndex, relativePosChange));
        currentLevelIndex++;
    }

    private IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("EndScreenDefeat");
    }

    private IEnumerator VictoryRoutine()
    {
        yield return null;
        UnityEngine.SceneManagement.SceneManager.LoadScene("EndScreenVictory");
    }

    private IEnumerator CamShakeRoutine()
    {
        float x = cam.transform.position.x + camShakeAxis.x;
        float y = cam.transform.position.y + camShakeAxis.y;
        Vector3 shakePos = new Vector3(UnityEngine.Random.Range(-x, x), UnityEngine.Random.Range(-y, y), 0f);
        yield return StartCoroutine(Utility.MoveGameObjectRoutine(cam.transform, shakePos, camShakeDuration, camShakeAnimCurve));
        camShakeCoroutine = null;
    }
}
