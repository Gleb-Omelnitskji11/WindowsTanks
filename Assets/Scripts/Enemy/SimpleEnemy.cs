using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleEnemy : IEnemyBehavior
{
    private readonly Vector2[] directions = new[] { Vector2.down, Vector2.left, Vector2.up, Vector2.right };
    private const float minTimeMoving = 1;
    private const float maxTimeMoving = 5;
    private const float moveSpeed = 5;

    private int curDirectionIndex = -1;
    private PatrollingData behaviorData;
    private Transform enemy;

    public void SetBehaviorData(string stateData)
    {
        if (stateData != null)
        {
            behaviorData = JsonUtility.FromJson<PatrollingData>(stateData);
            for (int i = 0; i < directions.Length; i++)
            {
                if (directions[i].Equals(behaviorData._direction))
                {
                    curDirectionIndex = i;
                    break;
                }
            }

            return;
        }

        behaviorData = new PatrollingData();
    }


    public void SetTransform(Transform obj)
    {
        enemy = obj;
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
        return JsonUtility.ToJson(behaviorData);
    }

    private void Patrolling()
    {
        behaviorData._timeMoving -= Time.deltaTime;
        if (behaviorData._timeMoving <= 0)
        {
            ChooseDirection();
        }

        Vector3 pos = enemy.position;
        pos += (Vector3)directions[curDirectionIndex] * (moveSpeed * Time.deltaTime);
        enemy.position = pos;
    }

    private void ChooseDirection()
    {
        int rnd = curDirectionIndex;
        behaviorData._timeMoving = Random.Range(minTimeMoving, maxTimeMoving);

        while (rnd == curDirectionIndex)
        {
            rnd = Random.Range(0, directions.Length);
        }

        curDirectionIndex = rnd;
        behaviorData._direction = directions[curDirectionIndex];
        float angle = Mathf.Atan2(behaviorData._direction.x, behaviorData._direction.y) * Mathf.Rad2Deg;
        enemy.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
    }
}

public class PatrollingData
{
    public Vector2 _direction;
    public float _timeMoving;
}