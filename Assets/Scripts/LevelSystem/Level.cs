using System;
using UnityEngine;

namespace Game.LevelSystem
{
    [Serializable]
    public class Level
    {
        public EnemySaveModel[] EnemySaveModels;
        public TransformModel PlayerSaveModel;
        public BulletSaveModel[] BulletModels;
    }

    [Serializable]
    public class EnemySaveModel
    {
        public EnemyType EnemyType;
        public TransformModel TransformModel;
        public string StateData;
    }

    [Serializable]
    public class BulletSaveModel
    {
        public string Tag;
        public TransformModel TransformModel;
    }

    [Serializable]
    public class TransformModel
    {
        public Vector2 Position;
        public Quaternion Rotation;
    }
}