using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Essentials.Core.CheatConsole
{
    [Serializable]
    public class ConsoleCommand
    {
        public string name;
        public string description;
        public bool validated;
        public bool autocompleteEnabled;
        public bool hidden;
        public List<string> arguments = new List<string>();

        [NonSerialized] public UnityEvent<string[]> onExecute = new UnityEvent<string[]>();
        [NonSerialized] public UnityEvent<string> onExecuteRaw = new UnityEvent<string>();
        [NonSerialized] public bool enabled = true;
    }
}