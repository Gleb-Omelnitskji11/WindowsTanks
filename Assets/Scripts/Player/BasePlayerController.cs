using System;
using UnityEngine;

[Serializable]
public abstract class BasePlayerController
{
    protected Transform Player;

    public void Init(Transform player)
    {
        Player = player;
    }

    public abstract void CheckControls();

    public virtual void OnShoot()
    {
        Shooting?.Invoke();
    }

    public event Action Shooting;
}