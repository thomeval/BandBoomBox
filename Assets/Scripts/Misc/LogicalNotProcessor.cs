using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Misc
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class LogicalNotProcessor : InputProcessor<float>
    {
        // [InitializeOnLoad] will call the static class constructor which
        // we use to call Register.
#if UNITY_EDITOR
        static LogicalNotProcessor()
        {
            Register();
        }
#endif

        // [RuntimeInitializeOnLoadMethod] will make sure that Register gets called
        // in the player on startup.
        // NOTE: This will also get called when going into play mode in the editor. In that
        //       case we get two calls to Register instead of one. We don't bother with that
        //       here. Calling RegisterProcessor twice here doesn't do any harm.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            // We don't supply a name here. The input system will take "JitterProcessor"
            // and automatically snip off the "Processor" suffix thus leaving us with
            // a name of "Jitter" (all this is case-insensitive).
            InputSystem.RegisterProcessor<LogicalNotProcessor>();
        }

        public override float Process(float value, InputControl control)
        {
            if (value >= 0.5f)
            {
                return 0.0f;
            }

            return 1.0f;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "LogicalNot()";
        }
    }
}
