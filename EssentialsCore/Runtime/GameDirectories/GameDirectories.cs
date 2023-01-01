using System.Collections.Generic;

namespace Essentials.Core.GameDirectories
{
    public static class GameDirectories
    {
        public static List<GameDirectory> gameDirectories = new List<GameDirectory>();

        public static GameDirectory[] GetAllGameDirectories()
        {
            List<GameDirectory> directories = new List<GameDirectory>();

            void AddSubDirectories(GameDirectory directory)
            {
                foreach (GameDirectory subDirectory in directory.subDirectories)
                {
                    directories.Add(subDirectory);
                    AddSubDirectories(subDirectory);
                }
            }

            foreach (GameDirectory gameDirectory in gameDirectories)
            {
                directories.Add(gameDirectory);
                AddSubDirectories(gameDirectory);
            }

            return directories.ToArray();
        }
    }

    public class GameDirectory
    {
        public string name;
        public string path;
        public string reference;
        public List<GameDirectory> subDirectories = new List<GameDirectory>();
    }
}