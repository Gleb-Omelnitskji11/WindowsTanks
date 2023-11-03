using System;
using Game.LevelSystem;
using UnityEngine;

namespace Game.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField]
        private Transform m_BulletSpawnPoint;

        private IEnemyBehavior m_EnemyBehavior;
        private EnemyType m_EnemyType;

        public void Init(IEnemyBehavior enemyBehavior, string stateData, EnemyType enemyType)
        {
            m_EnemyType = enemyType;
            m_EnemyBehavior = enemyBehavior;
            m_EnemyBehavior.SetTransform(transform);
            m_EnemyBehavior.SetBehaviorData(stateData);

            StartCoroutine(m_EnemyBehavior.DoState());
        }

        public EnemySaveModel GetEnemySaveModel()
        {
            EnemySaveModel enemySaveModel = new EnemySaveModel();

            Transform transform1 = transform;

            enemySaveModel.TransformModel = new TransformModel()
            {
                Position = transform1.position,
                Rotation = transform1.rotation
            };

            enemySaveModel.EnemyType = m_EnemyType;
            enemySaveModel.StateData = m_EnemyBehavior.GetStateData();
            return enemySaveModel;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("PlayerBullet"))
            {
                DeleteTank();
                return;
            }

            m_EnemyBehavior.OnCollisionEnter(collision.gameObject.tag);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            m_EnemyBehavior.OnCollisionStay(collision.gameObject.tag);
        }

        private void DeleteTank()
        {
            StopCoroutine(m_EnemyBehavior.DoState());
            gameObject.SetActive(false);
            Destroying?.Invoke(this);
            Destroy(gameObject);
        }

        public event Action<EnemyController> Destroying;
    }
}