using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private IEnemyBehavior _enemyBehavior;
    private EnemyType _enemyType;

    public void Init(IEnemyBehavior enemyBehavior, string stateData, EnemyType enemyType)
    {
        _enemyType = enemyType;
        _enemyBehavior = enemyBehavior;
        _enemyBehavior.SetTransform(transform);
        _enemyBehavior.SetBehaviorData(stateData);
        
        StartCoroutine(_enemyBehavior.DoState());
    }

    public EnemySaveModel GetEnemySaveModel()
    {
        EnemySaveModel enemySaveModel = new EnemySaveModel();
        enemySaveModel.transformModel = new TransformModel()
        {
            position = transform.position,
            rotation = transform.rotation
        };
        enemySaveModel.enemyType = _enemyType;
        enemySaveModel.stateData = _enemyBehavior.GetStateData();
        return enemySaveModel;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("PlayerBullet"))
        {
            DeleteTank();
            return;
        }
        
        _enemyBehavior.OnCollisionEnter(collision.gameObject.tag);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        _enemyBehavior.OnCollisionStay(collision.gameObject.tag);
    }

    private void DeleteTank()
    {
        StopCoroutine(_enemyBehavior.DoState());
        gameObject.SetActive(false);
        Destroying?.Invoke(this);
        Destroy(gameObject);
    }

    public event Action<EnemyController> Destroying;
}