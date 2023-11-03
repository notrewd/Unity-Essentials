using UnityEngine;

namespace Essentials.Core.Transformation
{
    public enum SnapDirection
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Backward
    }
    
    public static class Transformation
    {
        public static Vector3 GetSnapPrediction(GameObject gameObject, SnapDirection direction = SnapDirection.Down)
        {
            if (!Physics.Raycast(gameObject.transform.position, GetSnapDirection(direction), out RaycastHit hit)) return gameObject.transform.position;

            return direction switch
            {
                SnapDirection.Up => hit.point - Vector3.up * gameObject.GetComponent<Collider>().bounds.extents.y,
                SnapDirection.Down => hit.point + Vector3.up * gameObject.GetComponent<Collider>().bounds.extents.y,
                SnapDirection.Left => hit.point + Vector3.right * gameObject.GetComponent<Collider>().bounds.extents.x,
                SnapDirection.Right => hit.point - Vector3.right * gameObject.GetComponent<Collider>().bounds.extents.x,
                SnapDirection.Forward => hit.point - Vector3.forward * gameObject.GetComponent<Collider>().bounds.extents.z,
                SnapDirection.Backward => hit.point + Vector3.forward * gameObject.GetComponent<Collider>().bounds.extents.z,
                _ => Vector3.zero
            };
        }
        
        public static void SnapObjectTo(GameObject gameObject, SnapDirection direction = SnapDirection.Down)
        {
            Vector3 snapPrediction = GetSnapPrediction(gameObject, direction);
            gameObject.transform.position = snapPrediction;
        }
        
        public static void SnapObjectTo(Transform transform, SnapDirection direction = SnapDirection.Down) => SnapObjectTo(transform.gameObject, direction);

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
                _ => Vector3.zero
            };
        }
    }
}