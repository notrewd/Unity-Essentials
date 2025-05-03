using System.Collections.Generic;

namespace Essentials.Internal.GameDirectories
{
    /// <summary>
    /// Represents a directory within the game's directory structure, managed by the Game Directories editor.
    /// </summary>
    public class GameDirectory
    {
        /// <summary>
        /// The name of the directory.
        /// </summary>
        public string name;
        /// <summary>
        /// The full path of the directory relative to the root defined by the editor.
        /// </summary>
        public string path;
        /// <summary>
        /// The reference name used to access this directory in the generated static class. Can be null if not referenced.
        /// </summary>
        public string reference;
        /// <summary>
        /// A list of subdirectories contained within this directory.
        /// </summary>
        public List<GameDirectory> subDirectories = new List<GameDirectory>();
    }
}