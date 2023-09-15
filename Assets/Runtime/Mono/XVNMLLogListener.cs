#nullable enable

using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XVNML.Utilities.Diagnostics;

namespace XVNML2U.Mono
{
    public sealed class XVNMLLogListener : Singleton<XVNMLLogListener>
    {
        [SerializeField]
        private bool _receiveLogs = false;

        [Header("Log Configurations")]
        [SerializeField] 
        private bool _enableVerbose = true;
        
        [SerializeField] 
        private bool _enableWarning = true;
        
        [SerializeField] 
        private bool _enableError = true;

        [SerializeField]
        private bool _pauseGamePlayOnError = false;

        private IEnumerator? _coroutine;

        private void OnValidate()
        {
            if (_receiveLogs == false) return;
            if (!FindObjectsOfType<XVNMLModule>().Any())
            {
                Debug.LogWarning("There needs to at least be 1 XVNMLModule object to receive logs.");
                return;
            }

            if (!Application.isPlaying) return;

            if (Instance != null) Start();
        }

        private void Start()
        {
            if (_receiveLogs == false) return;

            _coroutine = LoggerListenerCycle();
            StartCoroutine(_coroutine);
        }

        private IEnumerator LoggerListenerCycle()
        {
            while (_receiveLogs)
            {
                XVNMLLogger.CollectLog(out XVNMLLogMessage? msg);

                if (msg == null)
                {
                    yield return null;
                    continue;
                }

                switch (msg.Level)
                {
                    case XVNMLLogLevel.Standard:
                        if (_enableVerbose) Debug.Log(msg.Message);
                        break;
                    case XVNMLLogLevel.Warning:
                        if (_enableWarning) Debug.LogWarning(msg.Message);
                        break;
                    case XVNMLLogLevel.Error:
                        if (_enableError) Debug.LogError(msg.Message);
                        #if UNITY_EDITOR
                        if (_pauseGamePlayOnError) EditorApplication.isPaused = true;
                        #endif
                        break;
                    default: break;
                }

                yield return null;
            }
        }
    }
}
