using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    private List<Bullet> pooledBullets = new List<Bullet>();

    public static BulletSpawner Instance;
    private const float bulletSpeed = 15f;

    private void Awake()
    {
        Instance = this;
    }

    public void Clear()
    {
        foreach (var bullet in pooledBullets)
        {
            bullet.TurnOff();
        }
    }

    public void Shoot(TransformModel transformModel, string tag)
    {
        var bullet = GetFreeBullet();
        var bulletTransform = bullet.transform;
        bulletTransform.position = transformModel.position;
        bulletTransform.rotation = transformModel.rotation;
        bullet.tag = tag;
        bullet.Shoot(bulletSpeed);
    }

    public BulletSaveModel[] GetBulletsData()
    {
        Bullet[] bullets = pooledBullets.Where(x => x.IsBusy).ToArray();
        BulletSaveModel[] bulletModels = new BulletSaveModel[bullets.Length];
        for (int i = 0; i < bullets.Length; i++)
        {
            Bullet bullet = bullets[i];
            Transform bulletTransform = bullet.transform;
            BulletSaveModel saveModel = new BulletSaveModel()
            {
                transformModel = new TransformModel()
                {
                    position = bulletTransform.position,
                    rotation = bulletTransform.rotation
                },
                tag = bullet.gameObject.tag
            };
            bulletModels[i] = saveModel;
        }

        return bulletModels;
    }

    private Bullet GetFreeBullet()
    {
        foreach (var bullet in pooledBullets)
        {
            if (!bullet.IsBusy)
            {
                bullet.gameObject.SetActive(true);
                return bullet;
            }
        }

        return CreateNew();
    }

    private Bullet CreateNew()
    {
        Bullet bullet = Instantiate(bulletPrefab);
        pooledBullets.Add(bullet);
        return bullet;
    }
}