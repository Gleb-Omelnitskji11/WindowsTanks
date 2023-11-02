using System;
using UnityEngine;

public class TetrisPlayerController : BasePlayerController
{
    private const float MoveSpeed = 20;

    public override void CheckControls()
    {
        Move();
        Shoot();
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0)
        {
            Player.position += new Vector3(h * MoveSpeed * Time.deltaTime, 0, 0);
            Player.rotation = Quaternion.Euler(0, 0, -90 * h);
        }
        else if (v != 0)
        {
            Player.position += new Vector3(0, v * MoveSpeed * Time.deltaTime, 0);
            Player.rotation = Quaternion.Euler(0, 0, 90 - 90 * v);
        }
    }

    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnShoot();
        }
    }
}