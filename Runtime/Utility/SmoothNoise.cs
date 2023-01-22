using UnityEngine;

namespace Gameframe.Procgen
{
    public static class SmoothNoise
    {
        public static float Value1d(float x, uint seed)
        {
            var x0 = Mathf.FloorToInt(x);
            var x1 = x0 + 1;

            var t = x - x0;

            var v0 = SquirrelEiserloh.Get1dNoiseZeroToOne(x0, seed);
            var v1 = SquirrelEiserloh.Get1dNoiseZeroToOne(x1, seed);

            return Mathf.Lerp(v0, v1, t);
        }

        public static float Value2d(Vector2 v, uint seed)
        {
            var x0 = (int) v.x;
            var y0 = (int) v.y;
            var x1 = x0 + 1;
            var y1 = y0 + 1;

            var tx = v.x - x0;
            var ty = v.y - y0;

            var v00 = SquirrelEiserloh.Get2dNoiseZeroToOne(x0, y0, seed);
            var v01 = SquirrelEiserloh.Get2dNoiseZeroToOne(x0, y1, seed);

            var v10 = SquirrelEiserloh.Get2dNoiseZeroToOne(x1, y0, seed);
            var v11 = SquirrelEiserloh.Get2dNoiseZeroToOne(x1, y1, seed);

            var edge1 = Mathf.Lerp(v00, v10, tx);
            var edge2 = Mathf.Lerp(v01, v11, tx);

            return Mathf.Lerp(edge1, edge2, ty);
        }

        public static float Value3d(Vector3 v, uint seed)
        {
            var x0 = (int) v.x;
            var y0 = (int) v.y;
            var z0 = (int) v.z;

            var x1 = x0 + 1;
            var y1 = y0 + 1;
            var z1 = z0 + 1;

            var tx = v.x - x0;
            var ty = v.y - y0;
            var tz = v.z - z0;

            var v000 = SquirrelEiserloh.Get3dNoiseZeroToOne(x0, y0, z0, seed);
            var v010 = SquirrelEiserloh.Get3dNoiseZeroToOne(x0, y1, z0, seed);
            var v001 = SquirrelEiserloh.Get3dNoiseZeroToOne(x0, y0, z1, seed);
            var v011 = SquirrelEiserloh.Get3dNoiseZeroToOne(x0, y1, z1, seed);

            var v100 = SquirrelEiserloh.Get3dNoiseZeroToOne(x1, y0, z0, seed);
            var v110 = SquirrelEiserloh.Get3dNoiseZeroToOne(x1, y1, z0, seed);
            var v101 = SquirrelEiserloh.Get3dNoiseZeroToOne(x1, y0, z1, seed);
            var v111 = SquirrelEiserloh.Get3dNoiseZeroToOne(x1, y1, z1, seed);

            //Lerp along all edges along the x axis
            var xEdge1 = Mathf.Lerp(v000, v100, tx);
            var xEdge2 = Mathf.Lerp(v010, v110, tx);
            var xEdge3 = Mathf.Lerp(v001, v101, tx);
            var xEdge4 = Mathf.Lerp(v011, v111, tx);

            //Lerp over y
            var yEdge1 = Mathf.Lerp(xEdge1, xEdge2, ty);
            var yEdge2 = Mathf.Lerp(xEdge3, xEdge4, ty);

            //Finally lerp over z
            return Mathf.Lerp(yEdge1, yEdge2, tz);
        }

        private static float Smooth(float t)
        {
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }
    }
}
