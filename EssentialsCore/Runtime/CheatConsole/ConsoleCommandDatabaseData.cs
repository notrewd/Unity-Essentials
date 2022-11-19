using System;
using System.Collections.Generic;

namespace Essentials.Core.CheatConsole
{
    [Serializable]
    public class ConsoleCommandDatabaseData
    {
        public List<ConsoleCommand> commands = new List<ConsoleCommand>();
    }
}
