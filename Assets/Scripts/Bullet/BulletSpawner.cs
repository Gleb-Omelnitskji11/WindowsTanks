using System.Collections.Generic;
using System.Linq;
using Game.LevelSystem;
using UnityEngine;

namespace Game.Bullet
{
    public class BulletSpawner : MonoBehaviour
    {
        [SerializeField]
        private Bullet m_BulletPrefab;

        private readonly List<Bullet> m_PooledBullets = new List<Bullet>();

        public static BulletSpawner Instance;
        private const float BulletSpeed = 15f;

        private void Awake()
        {
            Instance = this;
        }

        public void Clear()
        {
            foreach (var bullet in m_PooledBullets)
            {
                bullet.TurnOff();
            }
        }

        public void Shoot(TransformModel transformModel, string objTag)
        {
            var bullet = GetFreeBullet();
            var bulletTransform = bullet.transform;
            bulletTransform.position = transformModel.Position;
            bulletTransform.rotation = transformModel.Rotation;
            bullet.tag = objTag;
            bullet.Shoot(BulletSpeed);
        }

        public BulletSaveModel[] GetBulletsData()
        {
            Bullet[] bullets = m_PooledBullets.Where(x => x.IsBusy).ToArray();
            BulletSaveModel[] bulletModels = new BulletSaveModel[bullets.Length];

            for (int i = 0; i < bullets.Length; i++)
            {
                Bullet bullet = bullets[i];
                Transform bulletTransform = bullet.transform;

                BulletSaveModel saveModel = new BulletSaveModel()
                {
                    TransformModel = new TransformModel()
                    {
                        Position = bulletTransform.position,
                        Rotation = bulletTransform.rotation
                    },
                    Tag = bullet.gameObject.tag
                };

                bulletModels[i] = saveModel;
            }

            return bulletModels;
        }

        private Bullet GetFreeBullet()
        {
            foreach (var bullet in m_PooledBullets)
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
            Bullet bullet = Instantiate(m_BulletPrefab);
            m_PooledBullets.Add(bullet);
            return bullet;
        }
    }
}