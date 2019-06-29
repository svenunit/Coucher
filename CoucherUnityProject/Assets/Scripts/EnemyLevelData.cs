using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyLevelData", menuName = "EnemyLevelData", order = 1)]
public class EnemyLevelData : ScriptableObject
{
    [SerializeField] private Wave[] waves;
    public Wave[] Waves
    {
        get => waves;
        private set => waves = value;
    }
}

[System.Serializable]
public class Wave
{
    [SerializeField] private bool triggeredByTime;
    public bool TriggeredByTime
    {
        get => triggeredByTime;
        private set => triggeredByTime = value;
    }

    [Range(5f, 20f)] [SerializeField] private float timeTriggerMin;
    public float TimeTriggerMin
    {
        get => timeTriggerMin;
        private set => timeTriggerMin = value;
    }

    [Range(6f, 20f)] [SerializeField] private float timeTriggerMax;
    public float TimeTriggerMax
    {
        get => timeTriggerMax;
        private set => timeTriggerMax = value;
    }

    [SerializeField] private bool spawnEnemiesOverTime;
    public bool SpawnEnemiesOverTime
    {
        get => spawnEnemiesOverTime;
        private set => spawnEnemiesOverTime = value;
    }

    [Range(1f, 5f)] [SerializeField] private float enemySpawnDuration;
    public float EnemySpawnDuration
    {
        get => enemySpawnDuration;
        private set => enemySpawnDuration = value;
    }

    [SerializeField] private Enemy[] enemiesToSpawn;
    public Enemy[] EnemiesToSpawn
    {
        get => enemiesToSpawn;
        private set => enemiesToSpawn = value;
    }

}
