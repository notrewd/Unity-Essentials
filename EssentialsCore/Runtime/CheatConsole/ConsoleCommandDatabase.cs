using UnityEngine;

namespace Essentials.Core.CheatConsole
{
    [CreateAssetMenu(order = 250, fileName = "New Command Database", menuName = "Essentials/Cheat Console/Command Database")]
    public class ConsoleCommandDatabase : ScriptableObject
    {
        public string data;

        public ConsoleCommandDatabaseData GetData() => string.IsNullOrEmpty(data) ? new ConsoleCommandDatabaseData() : JsonUtility.FromJson<ConsoleCommandDatabaseData>(data);
    }
}