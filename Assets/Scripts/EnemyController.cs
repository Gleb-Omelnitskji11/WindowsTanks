using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;
    public float moveSpeed = 10;

    private Vector2[] directions = new[] { Vector2.down, Vector2.left, Vector2.up, Vector2.right };

    private int curDirectionIndex = -1;

    private void Start()
    {
        SetDirection();
        StartCoroutine(Move());
    }

    private void SetDirection()
    {
        int rnd = curDirectionIndex;
        
        while (rnd == curDirectionIndex)
        {
            rnd = Random.Range(0, directions.Length);
            Debug.Log(rnd);
        }

        curDirectionIndex = rnd;
        Vector2 currentDirection = directions[rnd];

        float angle = Mathf.Atan2(currentDirection.x, currentDirection.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
    }

    private IEnumerator Move()
    {
        while (true)
        {
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
            SetDirection();
        }
    }

    private void DeleteTank()
    {
        StopCoroutine(Move());
        Destroy(gameObject);
    }
    
    public event Action<EnemyController> Destroying;
}