using UnityEngine;

namespace MeshUI
{
    [System.Serializable]
    public class MeshNavigation
    {
        public MeshButton leftMeshButton;
        public MeshButton rightMeshButton;
        public MeshButton upMeshButton;
        public MeshButton downMeshButton;

        public MeshButton Navigate(Vector2 dir)
        {
            Vector2 cur = GetClosestDirection(dir);

            if (cur == Vector2.up)
                return upMeshButton;
            else if (cur == Vector2.down)
                return downMeshButton;
            else if (cur == Vector2.left)
                return leftMeshButton;
            else if (cur == Vector2.right)
                return rightMeshButton;

            throw new System.Exception("This direction don't exist in this context !");
        }

        private Vector2 GetClosestDirection(Vector2 direction)
        {
            // Directions to compare with
            Vector2[] cardinalDirections = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };

            float minAngleDifference = Mathf.Infinity;
            Vector2 closestDirection = Vector2.zero;

            // Find the closest cardinal direction
            foreach (Vector2 cardinalDir in cardinalDirections)
            {
                float angleDifference = Vector2.Angle(direction, cardinalDir);
                if (angleDifference < minAngleDifference)
                {
                    minAngleDifference = angleDifference;
                    closestDirection = cardinalDir;
                }
            }

            return closestDirection;
        }
    }
}
