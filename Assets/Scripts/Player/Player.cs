using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;

    private BasePlayerController _playerController;

    private void Start()
    {
        SetMovingSystem();
    }

    private void SetMovingSystem()
    {
        _playerController = PlayerMovementConfig.GetMovementSystem(MovementSystemType.Tetris);
        _playerController.Init(transform);
        _playerController.Shooting += OnShoot;
    }

    private void Update()
    {
        _playerController.CheckControls();
    }
    
    private void OnShoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = bulletSpawnPoint.up * bulletSpeed;
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