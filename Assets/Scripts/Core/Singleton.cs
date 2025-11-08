using UnityEngine;

namespace ReusableScripts.Core
{
    /// <summary>
    /// Generic singleton base class for Unity MonoBehaviour.
    /// Ensures only one instance exists and provides easy access to it.
    /// </summary>
    /// <typeparam name="T">The type of the singleton class</typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _isApplicationQuitting = false;
        private static readonly object _lock = new object();

        /// <summary>
        /// Public accessor for the singleton instance.
        /// Creates instance if it doesn't exist (lazy initialization).
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_isApplicationQuitting) return null;

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();

                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";

                            DontDestroyOnLoad(singletonObject);
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Check if singleton instance exists without creating one
        /// </summary>
        public static bool HasInstance => _instance != null && !_isApplicationQuitting;

        /// <summary>
        /// Override this to customize initialization behavior
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Mark as quitting to prevent creating new instances during shutdown
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            _isApplicationQuitting = true;
        }

        /// <summary>
        /// Reset instance when destroyed
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}