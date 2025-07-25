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
        private IEnumerator? _coroutine;

        private void OnValidate()
        {
            if (XVNML2USettingsUtil.ActiveProjectSettings.ReceiveLogs == false) return;
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
            if (XVNML2USettingsUtil.ActiveProjectSettings.ReceiveLogs == false) return;

            _coroutine = LoggerListenerCycle();
            StartCoroutine(_coroutine);
        }

        private IEnumerator LoggerListenerCycle()
        {
            while (XVNML2USettingsUtil.ActiveProjectSettings.ReceiveLogs)
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
                        if (XVNML2USettingsUtil.ActiveProjectSettings.EnableVerbose) Debug.Log(msg.Message);
                        break;
                    case XVNMLLogLevel.Warning:
                        if (XVNML2USettingsUtil.ActiveProjectSettings.EnableWarning) Debug.LogWarning(msg.Message);
                        break;
                    case XVNMLLogLevel.Error:
                        if (XVNML2USettingsUtil.ActiveProjectSettings.EnableError) Debug.LogError(msg.Message);
                        #if UNITY_EDITOR
                        if (XVNML2USettingsUtil.ActiveProjectSettings.PauseGamePlayOnError) EditorApplication.isPaused = true;
                        #endif
                        break;
                    default: break;
                }

                yield return null;
            }
        }
    }
}
