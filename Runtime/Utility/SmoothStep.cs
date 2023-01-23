namespace Gameframe.Procgen
{
    /// <summary>
    /// Smoothing methods utility class
    /// </summary>
    public static class SmoothStep
    {
        /// <summary>
        /// Commonly used smooth step method
        /// 3t2 - 2t3
        /// </summary>
        /// <param name="t">t</param>
        /// <returns>smoothed 0 to 1</returns>
        public static float Degree3(float t)
        {
            //Typical smooth step 3t2 - 2t3
            return t * t * (3 - 2 * t);
        }

        /// <summary>
        /// Smoothstep method recommended by Ken Perlin for noise generation
        /// First and second derivatives of this method are both 0 when t is zero or one.
        /// This prevents discontinuities when doing anything that might effectively show a derivative
        /// </summary>
        /// <param name="t">t</param>
        /// <returns>smoothed value 0 to 1</returns>
        public static float Degree5(float t)
        {
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }

        public static float Degree5Derivative(float t)
        {
            return 30f * t * t * (t * (t - 2f) + 1f);
        }
    }
}
