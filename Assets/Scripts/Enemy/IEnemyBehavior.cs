using System.Collections;
using UnityEngine;

namespace Game.Enemy
{
    public interface IEnemyBehavior
    {
        public void SetTransform(Transform transform);
        public void SetBehaviorData(string stageData);
        public void OnCollisionEnter(string objectTag);
        public void OnCollisionStay(string objectTag);
        public IEnumerator DoState();

        public string GetStateData();
    }
}