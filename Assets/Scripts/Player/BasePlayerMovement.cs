using System;
using UnityEngine;

namespace Game.Player
{
    [Serializable]
    public abstract class BasePlayerMovement
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
}