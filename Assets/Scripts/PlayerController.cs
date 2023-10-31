using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10;
    public float moveSpeed = 2;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("Enemy"))
        {
            Debug.Log("Do something here");
        }

        if (collision.gameObject.tag.Equals("Enemy"))
        {
            Debug.Log("Do something else here");
        }
    }

    private void Update()
    {
        Movement();
        Shooting();
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            DeleteTank();
        }
    }

    private void Movement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (h != 0)
        {
            transform.position += new Vector3(h * moveSpeed * Time.deltaTime, 0, 0);
            transform.rotation = Quaternion.Euler(0, 0, -90 * h);
        }
        else if (v != 0)
        {
            transform.position += new Vector3(0, v * moveSpeed * Time.deltaTime, 0);
            transform.rotation = Quaternion.Euler(0, 0, 90 - 90 * v);
        }
    }

    private void Shooting()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = bulletSpawnPoint.up * bulletSpeed;
        }
    }
    
    private void DeleteTank()
    {
        gameObject.SetActive(false);
        Destroying?.Invoke();
    }

    public event Action Destroying;
}