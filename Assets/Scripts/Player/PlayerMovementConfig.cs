using System;

namespace Game.Player
{
    public static class PlayerMovementConfig
    {
        public static BasePlayerMovement GetMovementSystem(MovementSystemType type)
        {
            switch (type)
            {
                case MovementSystemType.Tetris:
                    return new TetrisPlayerMovement();

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }

    public enum MovementSystemType
    {
        Tetris
    }
}