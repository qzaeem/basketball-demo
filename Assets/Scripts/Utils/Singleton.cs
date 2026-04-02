using UnityEngine;

namespace Basketball_Demo
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;

        public static T Instance => _instance;

        public static bool IsInitialized => _instance != null;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject); 
                return;
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

#if UNITY_EDITOR
        protected virtual void OnApplicationQuit()
        {
            _instance = null;
        }
#endif
    }
}