using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "FloatingRocksConfig", menuName = "Ditached/FloatingRocks", order = 0)]
    public class FloatingRocksConfig : ScriptableObject
    {
        public float floatSpeed = 1f;
        public float maxMovement = 1f;
        public float rotationSpeed = 15f;
    }
}