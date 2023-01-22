using UnityEngine;

namespace Gameframe.Procgen
{
    /// <summary>
    /// Random number generator that provides lots of different convenience methods relevant for game development
    /// </summary>
    public interface IRandomNumberGenerator
    {
        uint Seed { get; }
        uint NextUint();
        ushort NextUshort();
        byte NextByte();
        int NextInt();
        short NextShort();
        float NextFloatZeroToOne();
        float NextFloatNegOneToOne();
        float NextFloatRange(float min, float max);
        bool RollChance(float probabilityOfReturningTrue);
        Vector2 NextDirection2D();
        Vector3 NextDirection3D();
    }
}