using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static Event1<Enemy> EnemyDied { get; private set; }
    public static Event0 AllWavesDone { get; private set; }
    public static Event0 PlayerEnteredExit { get; private set; }
    public static Event1<(int levelIndex,Vector2 levelCenter)> NewLevelStarted { get; private set; }

    private void Awake()
    {
        InitEvents();
    }

    private void InitEvents()
    {
        EnemyDied = new Event1<Enemy>();
        AllWavesDone = new Event0();
        NewLevelStarted = new Event1<(int levelIndex, Vector2 levelCenter)>();
        PlayerEnteredExit = new Event0();
    }
}

