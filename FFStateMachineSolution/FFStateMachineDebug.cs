using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFStateMachineSolution
{
    public class FFStateMachineDebug
    {
        static bool enableConsoleWriteln = true;
        static List<FFStateMachineDebugMessage> lista = new List<FFStateMachineDebugMessage>();
        public static FFStateMachineEventManager evt = new FFStateMachineEventManager(
            new FFStateMachine("FFStateMachineDebug")
            );
        public static void Log(FFStateMachineDebugMessageType type, string title, string description, bool bShow)
        {
            FFStateMachineDebugMessage msg = new FFStateMachineDebugMessage(type, title, description);
            lista.Add(msg);
            if (bShow)
            {
                if (enableConsoleWriteln) Console.WriteLine(msg);
                evt.trigger(FFStateMachineEventType.normal, "onLog", null, new Dictionary<string, object>() { 
                {"msg",msg}
            });
            }
        }
    }
}
