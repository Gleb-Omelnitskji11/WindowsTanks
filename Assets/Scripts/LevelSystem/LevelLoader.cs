using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private TankSpawner m_TankSpawner;

    private void Awake()
    {
        NextLevel();
        Subscribe();
    }

    private void Subscribe()
    {
        m_TankSpawner.DestroyedAllEnemies += NextLevel;
    }

    private void NextLevel()
    {
        Level level = GetDefaultLevel();
        m_TankSpawner.LoadLevel(level);
    }

    private Level GetDefaultLevel()
    {
        Level defaultLevel = new Level
        {
            enemySaveModels = new EnemySaveModel[5]
        };

        for (int i = 0; i < defaultLevel.enemySaveModels.Length; i++)
        {
            defaultLevel.enemySaveModels[i] = new EnemySaveModel()
            {
                enemyType = EnemyType.simpleEnemy
            };
        }

        return defaultLevel;
    }
}