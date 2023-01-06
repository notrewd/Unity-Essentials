using UnityEngine;

namespace Essentials.Internal.GameDirectories
{
    public class GameDirectoriesSettingsObject : ScriptableObject
    {
        [TextArea]
        public string data = string.Empty;
    }
}