using System;
using Game.Bullet;
using Game.LevelSystem;
using UnityEngine;

namespace Game.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private Transform m_BulletSpawnPoint;

        private BasePlayerMovement m_PlayerMovement;

        private void Start()
        {
            SetMovingSystem();
        }

        private void SetMovingSystem()
        {
            m_PlayerMovement = PlayerMovementConfig.GetMovementSystem(MovementSystemType.Tetris);
            m_PlayerMovement.Init(transform);
            m_PlayerMovement.Shooting += OnShoot;
        }

        private void Update()
        {
            m_PlayerMovement.CheckControls();
        }

        private void OnShoot()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                TransformModel transformModel = new TransformModel()
                {
                    Position = m_BulletSpawnPoint.position,
                    Rotation = m_BulletSpawnPoint.rotation
                };

                BulletSpawner.Instance.Shoot(transformModel, "PlayerBullet");
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag.Equals("Enemy"))
            {
                DeleteTank();
            }
        }

        private void DeleteTank()
        {
            gameObject.SetActive(false);
            Destroying?.Invoke();
        }

        public event Action Destroying;
    }
}