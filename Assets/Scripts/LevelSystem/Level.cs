using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

[Serializable]
public class Level
{
    public EnemySaveModel[] enemySaveModels;
    public TransformModel playerSaveModel;
    public BulletSaveModel[] bulletModels;
}

[Serializable]
public class EnemySaveModel
{
    public EnemyType enemyType;
    public TransformModel transformModel;
    public string stateData;
}

[Serializable]
public class BulletSaveModel
{
    public string tag;
    public TransformModel transformModel;
}

[Serializable]
public class TransformModel
{
    public Vector2 position;
    public Quaternion rotation;
}