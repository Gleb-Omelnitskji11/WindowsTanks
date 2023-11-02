using System;

public static class PlayerMovementConfig
{
    public static BasePlayerController GetMovementSystem(MovementSystemType type)
    {
        switch (type)
        {
            case MovementSystemType.Tetris:
                return new TetrisPlayerController();

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}

public enum MovementSystemType
{
    Tetris
}