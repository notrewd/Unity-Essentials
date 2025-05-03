using UnityEngine;

namespace Essentials.Core.Transformation
{
    /// <summary>
    /// Defines the direction in which an object should be snapped.
    /// </summary>
    public enum SnapDirection
    {
        /// <summary>
        /// Snap in the upward direction (positive Y axis).
        /// </summary>
        Up,

        /// <summary>
        /// Snap in the downward direction (negative Y axis).
        /// </summary>
        Down,

        /// <summary>
        /// Snap in the left direction (negative X axis).
        /// </summary>
        Left,

        /// <summary>
        /// Snap in the right direction (positive X axis).
        /// </summary>
        Right,

        /// <summary>
        /// Snap in the forward direction (positive Z axis).
        /// </summary>
        Forward,

        /// <summary>
        /// Snap in the backward direction (negative Z axis).
        /// </summary>
        Backward
    }

    /// <summary>
    /// Provides utility methods for transforming and snapping objects to surfaces in different directions.
    /// </summary>
    public static class Transformation
    {
        /// <summary>
        /// Returns the predicted position where the object would snap in the specified direction.
        /// Performs a Raycast from the object's position in the given direction.
        /// Adjusts the hit point based on the object's collider bounds extents to place the object surface adjacent to the hit surface.
        /// </summary>
        /// <param name="gameObject">The GameObject to get the snap prediction for. Requires a Collider component.</param>
        /// <param name="direction">The direction to cast the ray for snapping. Defaults to Down.</param>
        /// <returns>The predicted position where the object would snap, or its current position if no surface is found or the object lacks a Collider.</returns>
        /// <remarks>Logs a warning if the GameObject does not have a Collider component.</remarks>
        public static Vector3 GetSnapPrediction(GameObject gameObject, SnapDirection direction = SnapDirection.Down)
        {
            Collider collider = gameObject.GetComponent<Collider>();
            if (collider == null)
            {
                Debug.LogWarning($"Essentials Core: GameObject '{gameObject.name}' is missing a Collider component for snapping.");
                return gameObject.transform.position;
            }

            if (!Physics.Raycast(gameObject.transform.position, GetSnapDirection(direction), out RaycastHit hit)) return gameObject.transform.position;

            return direction switch
            {
                SnapDirection.Up => hit.point - Vector3.up * collider.bounds.extents.y,
                SnapDirection.Down => hit.point + Vector3.up * collider.bounds.extents.y,
                SnapDirection.Left => hit.point + Vector3.right * collider.bounds.extents.x,
                SnapDirection.Right => hit.point - Vector3.right * collider.bounds.extents.x,
                SnapDirection.Forward => hit.point - Vector3.forward * collider.bounds.extents.z,
                SnapDirection.Backward => hit.point + Vector3.forward * collider.bounds.extents.z,
                _ => gameObject.transform.position // Should not happen with enum, but added for safety
            };
        }

        /// <summary>
        /// Snaps the specified GameObject to a surface in the specified direction by setting its position to the predicted snap point.
        /// </summary>
        /// <param name="gameObject">The GameObject to snap. Requires a Collider component.</param>
        /// <param name="direction">The direction to snap the object. Defaults to Down.</param>
        public static void SnapObjectTo(GameObject gameObject, SnapDirection direction = SnapDirection.Down)
        {
            Vector3 snapPrediction = GetSnapPrediction(gameObject, direction);
            gameObject.transform.position = snapPrediction;
        }

        /// <summary>
        /// Snaps the specified Transform's GameObject to a surface in the specified direction.
        /// </summary>
        /// <param name="transform">The Transform whose GameObject to snap. Requires a Collider component on the GameObject.</param>
        /// <param name="direction">The direction to snap the object. Defaults to Down.</param>
        public static void SnapObjectTo(Transform transform, SnapDirection direction = SnapDirection.Down) => SnapObjectTo(transform.gameObject, direction);

        /// <summary>
        /// Converts a SnapDirection enum value to its corresponding Vector3 direction vector.
        /// </summary>
        /// <param name="direction">The SnapDirection enum value.</param>
        /// <returns>A normalized Vector3 representing the global direction (e.g., Vector3.up for SnapDirection.Up).</returns>
        private static Vector3 GetSnapDirection(SnapDirection direction)
        {
            return direction switch
            {
                SnapDirection.Up => Vector3.up,
                SnapDirection.Down => Vector3.down,
                SnapDirection.Left => Vector3.left,
                SnapDirection.Right => Vector3.right,
                SnapDirection.Forward => Vector3.forward,
                SnapDirection.Backward => Vector3.back,
                _ => Vector3.zero // Default case, should not be reached
            };
        }
    }
}