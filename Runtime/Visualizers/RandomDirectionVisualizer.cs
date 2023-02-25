using UnityEngine;

namespace Gameframe.Procgen
{
    /// <summary>
    /// This component was written to test the NoiseRng's random direction for uniform distribution
    /// </summary>
    public class RandomDirectionVisualizer : MonoBehaviour
    {
        public DirectionDimensions dimensions = DirectionDimensions.Dir3D;

        public uint seed = 0;

        public int pointCount = 200;

        public float sphereRadius = 10;
        public float pointRadius = 0.1f;

        public Color sphereColor = Color.white;
        public Color pointColor = Color.white;

        private NoiseRng rand;

        public enum DirectionDimensions
        {
            Dir2D,
            Dir3D,
        };

        private void OnDrawGizmos()
        {
            if (rand == null)
            {
                rand = new NoiseRng(seed);
            }

            rand.ReSeed(seed, 0);

            Gizmos.color = sphereColor;
            Gizmos.DrawSphere(transform.position, sphereRadius);

            Gizmos.color = pointColor;
            for (int i = 0; i < pointCount; i++)
            {
                var dir = dimensions == DirectionDimensions.Dir3D ? rand.NextDirection3D() : (Vector3)rand.NextDirection2D();
                Gizmos.DrawSphere(transform.position + dir * sphereRadius, pointRadius);
            }
        }
    }
}
