using System.Linq;
using Essentials.Core.Transformation;
using UnityEditor;
using UnityEngine;

namespace Essentials.Internal.Transformation
{
    /// <summary>
    /// Provides menu items for object transformation functionalities in the Unity editor.
    /// </summary>
    public static class MenuTransformation
    {
        /// <summary>
        /// Snaps the selected objects to the ground.
        /// </summary>
        [MenuItem("Essentials/Transformation/Snap to/Ground", false, 100)]
        private static void SnapObjectToGround()
        {
            foreach (Transform transform in Selection.transforms)
            {
                Undo.RecordObject(transform, "Snap to Ground");
                Core.Transformation.Transformation.SnapObjectTo(transform);
            }
        }

        /// <summary>
        /// Snaps the selected objects upwards.
        /// </summary>
        [MenuItem("Essentials/Transformation/Snap to/Up", false, 101)]
        private static void SnapObjectToUp()
        {
            foreach (Transform transform in Selection.transforms)
            {
                Undo.RecordObject(transform, "Snap to Up");
                Core.Transformation.Transformation.SnapObjectTo(transform, SnapDirection.Up);
            }
        }

        /// <summary>
        /// Snaps the selected objects to the left.
        /// </summary>
        [MenuItem("Essentials/Transformation/Snap to/Left", false, 102)]
        private static void SnapObjectToLeft()
        {
            foreach (Transform transform in Selection.transforms)
            {
                Undo.RecordObject(transform, "Snap to Left");
                Core.Transformation.Transformation.SnapObjectTo(transform, SnapDirection.Left);
            }
        }

        /// <summary>
        /// Snaps the selected objects to the right.
        /// </summary>
        [MenuItem("Essentials/Transformation/Snap to/Right", false, 103)]
        private static void SnapObjectToRight()
        {
            foreach (Transform transform in Selection.transforms)
            {
                Undo.RecordObject(transform, "Snap to Right");
                Core.Transformation.Transformation.SnapObjectTo(transform, SnapDirection.Right);
            }
        }

        /// <summary>
        /// Snaps the selected objects forward.
        /// </summary>
        [MenuItem("Essentials/Transformation/Snap to/Forward", false, 104)]
        private static void SnapObjectToForward()
        {
            foreach (Transform transform in Selection.transforms)
            {
                Undo.RecordObject(transform, "Snap to Forward");
                Core.Transformation.Transformation.SnapObjectTo(transform, SnapDirection.Forward);
            }
        }

        /// <summary>
        /// Snaps the selected objects backward.
        /// </summary>
        [MenuItem("Essentials/Transformation/Snap to/Backward", false, 105)]
        private static void SnapObjectToBackward()
        {
            foreach (Transform transform in Selection.transforms)
            {
                Undo.RecordObject(transform, "Snap to Backward");
                Core.Transformation.Transformation.SnapObjectTo(transform, SnapDirection.Backward);
            }
        }

        /// <summary>
        /// Validation method for the "Snap to Ground" menu item.
        /// Enables the item only if there are selected objects with colliders.
        /// </summary>
        /// <returns>True if the menu item should be enabled, false otherwise.</returns>
        [MenuItem("Essentials/Transformation/Snap to/Ground", true)]
        private static bool SnapObjectToGroundValidation()
        {
            if (Selection.transforms.Length == 0) return false;
            if (Selection.transforms.Any(transform => transform.GetComponent<Collider>() == null)) return false;

            return true;
        }

        /// <summary>
        /// Validation method for the "Snap to Up" menu item.
        /// </summary>
        /// <returns>True if the menu item should be enabled, false otherwise.</returns>
        [MenuItem("Essentials/Transformation/Snap to/Up", true)]
        private static bool SnapObjectToUpValidation() => Selection.transforms.Length != 0 && Selection.transforms.All(transform => transform.GetComponent<Collider>() != null);

        /// <summary>
        /// Validation method for the "Snap to Left" menu item.
        /// </summary>
        /// <returns>True if the menu item should be enabled, false otherwise.</returns>
        [MenuItem("Essentials/Transformation/Snap to/Left", true)]
        private static bool SnapObjectToLeftValidation() => Selection.transforms.Length != 0 && Selection.transforms.All(transform => transform.GetComponent<Collider>() != null);

        /// <summary>
        /// Validation method for the "Snap to Right" menu item.
        /// </summary>
        /// <returns>True if the menu item should be enabled, false otherwise.</returns>
        [MenuItem("Essentials/Transformation/Snap to/Right", true)]
        private static bool SnapObjectToRightValidation() => Selection.transforms.Length != 0 && Selection.transforms.All(transform => transform.GetComponent<Collider>() != null);

        /// <summary>
        /// Validation method for the "Snap to Forward" menu item.
        /// </summary>
        /// <returns>True if the menu item should be enabled, false otherwise.</returns>
        [MenuItem("Essentials/Transformation/Snap to/Forward", true)]
        private static bool SnapObjectToForwardValidation() => Selection.transforms.Length != 0 && Selection.transforms.All(transform => transform.GetComponent<Collider>() != null);

        /// <summary>
        /// Validation method for the "Snap to Backward" menu item.
        /// </summary>
        /// <returns>True if the menu item should be enabled, false otherwise.</returns>
        [MenuItem("Essentials/Transformation/Snap to/Backward", true)]
        private static bool SnapObjectToBackwardValidation() => Selection.transforms.Length != 0 && Selection.transforms.All(transform => transform.GetComponent<Collider>() != null);
    }
}