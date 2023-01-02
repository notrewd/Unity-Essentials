using System;
using System.Collections.Generic;

namespace Essentials.Core.GameDirectories
{
    public static class GameDirectories
    {
        public static List<GameDirectory> gameDirectories = new List<GameDirectory>();

        public static GameDirectory FindGameDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            string[] directories = Array.Empty<string>();

            if (path.Contains("\\")) path = path.Replace("\\", "/");
            if (path.Contains("/")) directories = path.Split('/');
            else directories = new string[] { path };

            if (directories.Length == 0) return null;

            foreach (string directory in directories)
            {
                if (string.IsNullOrWhiteSpace(directory)) return null;
                if (directory.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0) return null;
            }

            GameDirectory FindSubDirectory(GameDirectory directory, string[] subDirectories)
            {
                if (subDirectories.Length == 0) return directory;

                GameDirectory subDirectory = directory.subDirectories.Find(x => x.name == subDirectories[0]);

                if (subDirectory == null) return null;

                return FindSubDirectory(subDirectory, subDirectories[1..]);
            }

            GameDirectory gameDirectory = Core.GameDirectories.GameDirectories.gameDirectories.Find(x => x.name == directories[0]);

            if (gameDirectory == null) return null;

            GameDirectory foundDirectory = FindSubDirectory(gameDirectory, directories[1..]);

            if (foundDirectory == null) return null;

            return foundDirectory;
        }

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