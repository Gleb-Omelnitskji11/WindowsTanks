using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Enemy
{
    public class SimpleEnemy : IEnemyBehavior
    {
        private readonly Vector2[] m_Directions = new[] { Vector2.down, Vector2.left, Vector2.up, Vector2.right };
        private const float MinTimeMoving = 1;
        private const float MaxTimeMoving = 5;
        private const float MoveSpeed = 5;

        private int m_CurDirectionIndex = -1;
        private PatrollingData m_BehaviorData;
        private Transform m_Enemy;

        public void SetBehaviorData(string stateData)
        {
            if (stateData != null)
            {
                m_BehaviorData = JsonUtility.FromJson<PatrollingData>(stateData);

                for (int i = 0; i < m_Directions.Length; i++)
                {
                    if (m_Directions[i].Equals(m_BehaviorData.Direction))
                    {
                        m_CurDirectionIndex = i;
                        break;
                    }
                }

                return;
            }

            m_BehaviorData = new PatrollingData();
        }


        public void SetTransform(Transform obj)
        {
            m_Enemy = obj;
        }

        public void OnCollisionEnter(string objectTag)
        {
            if (objectTag.Equals("Enemy") || objectTag.Equals("Player") || objectTag.Equals("Border"))
            {
                ChooseDirection();
            }
        }

        public void OnCollisionStay(string objectTag)
        {
            if (objectTag.Equals("Enemy") || objectTag.Equals("Border"))
            {
                ChooseDirection();
            }
        }

        public IEnumerator DoState()
        {
            while (true)
            {
                Patrolling();
                yield return null;
            }
        }

        public string GetStateData()
        {
            return JsonUtility.ToJson(m_BehaviorData);
        }

        private void Patrolling()
        {
            m_BehaviorData.TimeMoving -= Time.deltaTime;

            if (m_BehaviorData.TimeMoving <= 0)
            {
                ChooseDirection();
            }

            Vector3 pos = m_Enemy.position;
            pos += (Vector3)m_Directions[m_CurDirectionIndex] * (MoveSpeed * Time.deltaTime);
            m_Enemy.position = pos;
        }

        private void ChooseDirection()
        {
            int rnd = m_CurDirectionIndex;
            m_BehaviorData.TimeMoving = Random.Range(MinTimeMoving, MaxTimeMoving);

            while (rnd == m_CurDirectionIndex)
            {
                rnd = Random.Range(0, m_Directions.Length);
            }

            m_CurDirectionIndex = rnd;
            m_BehaviorData.Direction = m_Directions[m_CurDirectionIndex];
            float angle = Mathf.Atan2(m_BehaviorData.Direction.x, m_BehaviorData.Direction.y) * Mathf.Rad2Deg;
            m_Enemy.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        }
    }

    public class PatrollingData
    {
        public Vector2 Direction;
        public float TimeMoving;
    }
}