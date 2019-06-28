using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static Event1<Enemy> EnemyDied { get; private set; }
    public static Event0 AllEnemiesDead { get; private set; }

    private void Awake()
    {
        InitEvents();
    }

    private void InitEvents()
    {
        EnemyDied = new Event1<Enemy>();
        AllEnemiesDead = new Event0();
    }
}

