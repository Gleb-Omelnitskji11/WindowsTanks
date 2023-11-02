using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    private float moveSpeed = 10;
    private const float minTimeMoving = 5;
    private const float maxTimeMoving = 20;

    private Vector2[] directions = new[] { Vector2.down, Vector2.left, Vector2.up, Vector2.right };

    private int curDirectionIndex = -1;
    private IEnemyBehavior _enemyBehavior;
    private PatrollingData _patrollingData;

    public void Init(PatrollingData patrollingData)
    {
        _patrollingData = patrollingData;
        if (_patrollingData == null)
        {
            _patrollingData = new PatrollingData();
            ChooseDirection();
        }

        StartCoroutine(Patrolling());
    }

    public void SetBehavior(IEnemyBehavior enemyBehavior)
    {
        _enemyBehavior = enemyBehavior;
    }

    private void SetLoadData(TransformModel transformModel)
    {
        transform.position = transformModel.position;
        transform.rotation = transformModel.rotation;
    }

    private void ChooseDirection()
    {
        int rnd = curDirectionIndex;
        _patrollingData.timeMoving = Random.Range(minTimeMoving, maxTimeMoving);

        while (rnd == curDirectionIndex)
        {
            rnd = Random.Range(0, directions.Length);
        }

        curDirectionIndex = rnd;
        _patrollingData.direction = directions[curDirectionIndex];
        float angle = Mathf.Atan2(_patrollingData.direction.x, _patrollingData.direction.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
    }

    private IEnumerator Patrolling()
    {
        while (true)
        {
            _patrollingData.timeMoving -= Time.deltaTime;
            if (_patrollingData.timeMoving == 0)
            {
                ChooseDirection();
            }

            Transform objectTransform = transform;
            Vector3 pos = objectTransform.position;
            pos += (Vector3)directions[curDirectionIndex] * (moveSpeed * Time.deltaTime);
            objectTransform.position = pos;
            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("PlayerBullet"))
        {
            DeleteTank();
        }

        if (collision.gameObject.tag.Equals("Enemy") || collision.gameObject.tag.Equals("Player") ||
            collision.gameObject.tag.Equals("Border"))
        {
            ChooseDirection();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            ChooseDirection();
        }
    }

    private void DeleteTank()
    {
        StopCoroutine(Patrolling());
        Destroying?.Invoke(this);
        Destroy(gameObject);
    }

    public event Action<EnemyController> Destroying;
}

public class SimpleEnemy : IEnemyBehavior
{
}

public class PatrollingData
{
    public Vector2 direction;
    public float timeMoving;
}