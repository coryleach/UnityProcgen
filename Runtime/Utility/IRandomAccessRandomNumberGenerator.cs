namespace Gameframe.Procgen
{
    /// <summary>
    /// Random number generator that provides random access to any position in its random number sequence
    /// </summary>
    public interface IRandomAccessRandomNumberGenerator : IRandomNumberGenerator
    {
        int Position { get; set; }
    }
}