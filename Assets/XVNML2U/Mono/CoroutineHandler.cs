using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using XVNML2U.Mono;

using Debug = UnityEngine.Debug;

/// <summary>
/// This sealed class assisted other classes that do not inherit from Monobehaviour
/// and need to run a coroutine. It's highly recommended to not use this class's methods unless you have to.
/// </summary>
namespace XVNML2U.Mono
{
    public sealed class CoroutineHandler : Singleton<CoroutineHandler>
    {
        static Dictionary<IEnumerator, int> CoroutineLog = new Dictionary<IEnumerator, int>();
        public static int Size => CoroutineLog.Count;
        public static void Execute(IEnumerator enumerator)
        {
            if (IsNull == true) return;
            if (enumerator == null) return;
            if (CoroutineLog.ContainsKey(enumerator)) return;

            Instance.StartCoroutine(enumerator);
            CoroutineLog.Add(enumerator, enumerator.GetHashCode());
        }

        public static void Halt(IEnumerator enumerator)
        {
            if (!IsNull && enumerator != null && CoroutineLog.ContainsKey(enumerator))
            {
                Instance.StopCoroutine(enumerator);
                CoroutineLog.Remove(enumerator);
            }
        }

        public static void ClearRoutines()
        {
            CoroutineLog.Clear();
            Instance.StopAllCoroutines();
        }
    }
}
