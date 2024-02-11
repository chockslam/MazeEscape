using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] ObjectPooler objectPooler;
    [SerializeField] UIManager uiManager;
    [SerializeField] List<EnemyData> enemiesToSpawn;

    Player m_player;
    Enemy[] m_enemies;

    Observer<GameEvent> m_eventObserver;

    void Awake()
    {
        m_eventObserver = new();

        uint numberOfEnemies = 0;
        foreach (EnemyData enemyData in enemiesToSpawn) 
        {
            numberOfEnemies += enemyData.number;
        }
        m_enemies = new Enemy[numberOfEnemies];
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        GameStart();
    }

    public void RestartLevel() 
    {
        StartCoroutine(RestartLevelCoroutine());
    }

    public IEnumerator RestartLevelCoroutine()
    {
        m_eventObserver.NotifySubscribers(new GameEvent(EventType.GameRestarting, null));
        ReturnPlayerToPool();
        ReturnEnemiesToPool();
        yield return mazeGenerator.RegenerateMaze();
        SpawnPlayerAtStart();
        SpawnEnemies();
        m_eventObserver.NotifySubscribers(new GameEvent(EventType.GamePlaying, null));
    }

    public void GameStart() 
    {
        StartCoroutine(GameStartCoroutine());
    }

    IEnumerator GameStartCoroutine()
    {
        uiManager.SetEventObserver(m_eventObserver);
        m_eventObserver.NotifySubscribers(new GameEvent(EventType.Initializing, null));
        yield return mazeGenerator.Initialize(objectPooler);
        SpawnPlayerAtStart();
        SpawnEnemies();
        m_eventObserver.NotifySubscribers(new GameEvent(EventType.GamePlaying, null));
    }

    void SpawnPlayerAtStart()
    {
        Vector2Int mazeStartPosition = mazeGenerator.GetStartPosition();
        Vector3 startWorldPosition = new(mazeStartPosition.x, 0, mazeStartPosition.y);
        GameObject playerObj = objectPooler.GetPooledObject(PoolTag.Player, startWorldPosition, Quaternion.identity, null);
        m_player = playerObj.GetComponent<Player>();
        
        if (m_player == null)
        {
            Debug.LogError("Spawned object does not have a Player component");
            return;
        }

        m_player.SetEventObserver(m_eventObserver);
        m_player.InitializeInputHanlder(uiManager.GetJoystick());
        Vector2Int mazeEndPosition = mazeGenerator.GetEndPosition();

        m_player.SetMazeEndPosition(new Vector3(mazeEndPosition.x, 0, mazeEndPosition.y));
    }

    void SpawnEnemies()
    {
        uint numberOfEnemies = 0;
        foreach (EnemyData enemyData in enemiesToSpawn) 
        {
            for (int i = 0; i < enemyData.number; ++i)
            {
                Vector2Int spawnPosition = mazeGenerator.FindRandomSpawnPosition();
                Vector3 worldPosition = new Vector3(spawnPosition.x, 0, spawnPosition.y);
                GameObject enemyObj = objectPooler.GetPooledObject(enemyData.enemyPoolTag, worldPosition, Quaternion.identity, null);

                if (!enemyObj.TryGetComponent<Enemy>(out var enemy))
                {
                    Debug.LogError($"Spawned enemy GameObject does not have an Enemy component attached. PoolTag of the spawned enemy is: {enemyData.enemyPoolTag}");
                    break;
                }

                enemy.SetEventObserver(m_eventObserver);
                enemy.SetPlayer(m_player);
                enemy.SetGrid(mazeGenerator.GetGrid());
                m_enemies[numberOfEnemies] = enemy;
                ++numberOfEnemies;
            }
        }
        
    }

    void ReturnPlayerToPool()
    {
        if (m_player != null) objectPooler.ReturnObjectToPool(PoolTag.Player, m_player.gameObject);
    }

    void ReturnEnemiesToPool()
    {
        foreach (var enemy in m_enemies)
        {
            if (enemy != null) objectPooler.ReturnObjectToPool(PoolTag.Enemy1, enemy.gameObject);
        }
    }

    [Serializable]
    public class EnemyData 
    {
        public PoolTag enemyPoolTag;
        public uint number;

    }
}
