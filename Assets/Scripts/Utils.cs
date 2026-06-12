using UnityEngine;

namespace ChoosingDirection.Utils
{
    public static class DirectionUtils
    {
        public static Vector3 GetRandomDir()
        {
            return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }
    }
}
