using System.Collections.Generic;

namespace Essentials.Internal.GameDirectories
{
    public class GameDirectory
    {
        public string name;
        public string path;
        public string reference;
        public List<GameDirectory> subDirectories = new List<GameDirectory>();
    }
}