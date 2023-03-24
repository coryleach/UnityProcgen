using System;
using UnityEngine;

namespace Gameframe.Procgen
{
    /// <summary>
    /// Noise based random number generator Struct version
    /// If this will be passed as a method arg use class version to avoid generating duplicate values
    ///
    /// Allows for fast random-access within a sequence of generated numbers
    /// Based on GDC 2017 talk and code by Squirrel Eiserloh
    /// </summary>
    public struct RandomGeneratorStruct : IRandomAccessRandomNumberGenerator
    {
        private uint seed;
        private int position;

        /// <summary>
        /// Initialize random number generator with a given seed
        /// </summary>
        /// <param name="seed">see for the random number generator</param>
        public RandomGeneratorStruct(uint seed)
        {
            this.seed = seed;
            position = 0;
        }

        /// <summary>
        /// Current position in the number generator sequence
        /// </summary>
        public int Position
        {
            get => position;
            set => position = value;
        }

        /// <summary>
        /// Current Seed
        /// </summary>
        public uint Seed => seed;

        /// <summary>
        /// Reseed the number generator with a new seed and position
        /// </summary>
        /// <param name="seed">seed</param>
        /// <param name="position">position in number generation sequence</param>
        public void ReSeed(uint seed, int position = 0)
        {
            this.seed = seed;
            this.position = position;
        }

        #region Rand Methods

        /// <summary>
        /// Next random unsigned integer
        /// </summary>
        /// <returns>next random unsigned integer in the current sequence</returns>
        public uint NextUint()
        {
            return SquirrelEiserloh.Get1dNoiseUint(position++, seed);
        }

        /// <summary>
        /// Next random unsigned short
        /// </summary>
        /// <returns>next random unsigned short in the current sequence</returns>
        public ushort NextUshort()
        {
            return unchecked((ushort) NextUint());
        }

        /// <summary>
        /// Next random byte
        /// </summary>
        /// <returns>next random byte in the current sequence</returns>
        public byte NextByte()
        {
            return unchecked((byte) NextUint());
        }

        /// <summary>
        /// Next random integer
        /// </summary>
        /// <returns>next random integer in the current sequence</returns>
        public int NextInt()
        {
            return unchecked((int) NextUint());
        }

        /// <summary>
        /// Next random short
        /// </summary>
        /// <returns>next random short in the current sequence</returns>
        public short NextShort()
        {
            return unchecked((short) NextUint());
        }

        /// <summary>
        /// Next random float in the range 0 to 1
        /// </summary>
        /// <returns>Next random float in the range 0 to 1 (inclusive, inclusive)</returns>
        public float NextFloatZeroToOne()
        {
            return SquirrelEiserloh.Get1dNoiseZeroToOne(position++, seed);
        }

        /// <summary>
        /// Next random float in the range -1 to 1
        /// </summary>
        /// <returns>Next random float in the range -1 to 1 (inclusive, inclusive)</returns>
        public float NextFloatNegOneToOne()
        {
            return SquirrelEiserloh.Get1dNoiseNegOneToOne(position++, seed);
        }

        /// <summary>
        /// Next random float in the range min to max
        /// </summary>
        /// <param name="min">min return value</param>
        /// <param name="max">max return value</param>
        /// <returns>Next random float in the range min to max (inclusive, inclusive)</returns>
        public float NextFloatRange(float min, float max)
        {
            var v = SquirrelEiserloh.Get1dNoiseZeroToOne(position++, seed);
            return min + (max - min) * v;
        }

        /// <summary>
        /// Next random double in the range 0 to 1
        /// </summary>
        /// <returns>Next random double in the range 0 to 1 (inclusive, inclusive)</returns>
        public double NextDoubleZeroToOne()
        {
            return SquirrelEiserloh.Get1dNoiseZeroToOne_Double(position++, seed);
        }

        /// <summary>
        /// Next random double in the range -1 to 1
        /// </summary>
        /// <returns>Next random double in the range -1 to 1 (inclusive, inclusive)</returns>
        public double NextDoubleNegOneToOne()
        {
            return SquirrelEiserloh.Get1dNoiseNegOneToOne_Double(position++, seed);
        }

        /// <summary>
        /// Next random double in the range min to max
        /// </summary>
        /// <param name="min">min return value</param>
        /// <param name="max">max return value</param>
        /// <returns>Next random double in the range min to max (inclusive, inclusive)</returns>
        public double NextDoubleRange(double min, double max)
        {
            var v = SquirrelEiserloh.Get1dNoiseZeroToOne_Double(position++, seed);
            return min + (max - min) * v;
        }

        /// <summary>
        /// Next random integer in the range min to max
        /// </summary>
        /// <param name="min">min return value</param>
        /// <param name="max">max return value</param>
        /// <returns>Next random int in the range min to max (inclusive, inclusive)</returns>
        public int NextIntRange(int min, int max)
        {
            return min + (int) ((1 + max - min) * NextFloatZeroToOne());
        }

        /// <summary>
        /// Rolls for a true for false with the given probability
        /// </summary>
        /// <param name="probabilityOfReturningTrue">probably that this function will return true</param>
        /// <returns>true or false</returns>
        public bool RollChance(float probabilityOfReturningTrue)
        {
            var v = SquirrelEiserloh.Get1dNoiseZeroToOne(position++, seed);
            return v < probabilityOfReturningTrue;
        }

        /// <summary>
        /// Next uniformly distributed randomized 2d direction
        /// </summary>
        /// <returns>Random Normalized Vector2</returns>
        public Vector2 NextDirection2D()
        {
            var theta = NextFloatRange(0, 2 * Mathf.PI);
            var v = new Vector2
            {
                x = Mathf.Cos(theta),
                y = Mathf.Sin(theta)
            };
            return v;
        }

        /// <summary>
        /// Next random 2D int vector in 4 possible directions
        /// </summary>
        /// <returns>Int Direction vector in one of 4 directions</returns>
        public Vector2Int Next4DirectionInt2D()
        {
            switch (NextIntRange(0, 3))
            {
                case 0:
                    return new Vector2Int(1, 0);
                case 1:
                    return new Vector2Int(0, 1);
                case 2:
                    return new Vector2Int(-1, 0);
                case 3:
                    return new Vector2Int(0, -1);
                default:
                    throw new Exception("invalid value");
            }
        }

        /// <summary>
        /// Next random 2D int vector in 8 possible directions
        /// </summary>
        /// <returns>Int Direction vector in one of 8 directions</returns>
        public Vector2Int Next8DirectionInt2D()
        {
            switch (NextIntRange(0, 7))
            {
                case 0:
                    return new Vector2Int(1, 0);
                case 1:
                    return new Vector2Int(0, 1);
                case 2:
                    return new Vector2Int(-1, 0);
                case 3:
                    return new Vector2Int(0, -1);
                case 4:
                    return new Vector2Int(1, 1);
                case 5:
                    return new Vector2Int(1, -1);
                case 6:
                    return new Vector2Int(-1, 1);
                case 7:
                    return new Vector2Int(-1, -1);
                default:
                    throw new Exception("invalid value");
            }
        }

        /// <summary>
        /// Next random vector in one of 4 directions
        /// </summary>
        /// <returns>Random 2D vector in one of 4 directions</returns>
        public Vector2 Next4Direction2D()
        {
            switch (NextIntRange(0, 3))
            {
                case 0:
                    return Vector2.left;
                case 1:
                    return Vector2.right;
                case 2:
                    return Vector2.up;
                case 3:
                    return Vector2.down;
                default:
                    throw new Exception("invalid value");
            }
        }

        /// <summary>
        /// Next random 2D vector in 8 possible directions
        /// </summary>
        /// <returns>Normalized vector in one of 8 directions</returns>
        public Vector2 Next8Direction2D()
        {
            switch (NextIntRange(0, 7))
            {
                case 0:
                    return new Vector2(1, 0);
                case 1:
                    return new Vector2(0, 1);
                case 2:
                    return new Vector2(-1, 0);
                case 3:
                    return new Vector2(0, -1);
                case 4:
                    return new Vector2(1, 1).normalized;
                case 5:
                    return new Vector2(1, -1).normalized;
                case 6:
                    return new Vector2(-1, 1).normalized;
                case 7:
                    return new Vector2(-1, -1).normalized;
                default:
                    throw new Exception("invalid value");
            }
        }

        /// <summary>
        /// Next uniformly distributed randomized 3d direction
        /// </summary>
        /// <returns>Random Normalized Vector3</returns>
        public Vector3 NextDirection3D()
        {
            //Random Polar coordinates with uniform distribution
            var phi = NextFloatRange(0, 2 * Mathf.PI);
            var theta = Mathf.Acos(NextFloatNegOneToOne());

            var v = new Vector3
            {
                x = Mathf.Cos(phi) * Mathf.Sin(theta),
                y = Mathf.Sin(phi) * Mathf.Sin(theta),
                z = Mathf.Cos(theta)
            };

            return v;
        }

        #endregion
    }

    /// <summary>
    /// Noise based random number generator
    /// Allows for fast random-access within a sequence of generated numbers
    /// Based on GDC 2017 talk and code by Squirrel Eiserloh
    /// </summary>
    public class RandomGenerator : IRandomAccessRandomNumberGenerator
    {
        private RandomGeneratorStruct _randomGeneratorStruct;

        /// <summary>
        /// Initialize random number generator with a given seed
        /// </summary>
        /// <param name="seed">see for the random number generator</param>
        public RandomGenerator(uint seed)
        {
            _randomGeneratorStruct = new RandomGeneratorStruct(seed);
        }

        /// <summary>
        /// Seeds using DateTime.UtcNow.Ticks
        /// </summary>
        public RandomGenerator()
        {
            _randomGeneratorStruct = new RandomGeneratorStruct((uint) DateTime.UtcNow.Ticks);
        }

        /// <inheritdoc cref="RandomGeneratorStruct.Position"/>
        public int Position
        {
            get => _randomGeneratorStruct.Position;
            set => _randomGeneratorStruct.Position = value;
        }

        /// <inheritdoc cref="RandomGeneratorStruct.Seed"/>
        public uint Seed => _randomGeneratorStruct.Seed;

        /// <inheritdoc cref="RandomGeneratorStruct.ReSeed"/>
        public void ReSeed(uint seed, int position = 0) => _randomGeneratorStruct.ReSeed(seed, position);

        #region Rand Methods

        /// <inheritdoc cref="RandomGeneratorStruct.NextUint"/>
        public uint NextUint() => _randomGeneratorStruct.NextUint();

        /// <inheritdoc cref="RandomGeneratorStruct.NextUshort"/>
        public ushort NextUshort() => _randomGeneratorStruct.NextUshort();

        /// <inheritdoc cref="RandomGeneratorStruct.NextByte"/>
        public byte NextByte() => _randomGeneratorStruct.NextByte();

        /// <inheritdoc cref="RandomGeneratorStruct.NextInt"/>
        public int NextInt() => _randomGeneratorStruct.NextInt();

        /// <inheritdoc cref="RandomGeneratorStruct.NextShort"/>
        public short NextShort() => _randomGeneratorStruct.NextShort();

        /// <inheritdoc cref="RandomGeneratorStruct.NextFloatZeroToOne"/>
        public float NextFloatZeroToOne() => _randomGeneratorStruct.NextFloatZeroToOne();

        /// <inheritdoc cref="RandomGeneratorStruct.NextFloatNegOneToOne"/>
        public float NextFloatNegOneToOne() => _randomGeneratorStruct.NextFloatNegOneToOne();

        /// <inheritdoc cref="RandomGeneratorStruct.NextFloatRange"/>
        public float NextFloatRange(float min, float max) => _randomGeneratorStruct.NextFloatRange(min, max);

        /// <inheritdoc cref="RandomGeneratorStruct.NextDoubleZeroToOne"/>
        public double NextDoubleZeroToOne()
        {
            return _randomGeneratorStruct.NextDoubleZeroToOne();
        }

        /// <inheritdoc cref="RandomGeneratorStruct.NextDoubleNegOneToOne"/>
        public double NextDoubleNegOneToOne()
        {
            return _randomGeneratorStruct.NextDoubleNegOneToOne();
        }

        /// <inheritdoc cref="RandomGeneratorStruct.NextDoubleRange"/>
        public double NextDoubleRange(double min, double max)
        {
            return _randomGeneratorStruct.NextDoubleRange(min, max);
        }

        /// <inheritdoc cref="RandomGeneratorStruct.NextIntRange"/>
        public int NextIntRange(int min, int max) => _randomGeneratorStruct.NextIntRange(min, max);

        /// <inheritdoc cref="RandomGeneratorStruct.RollChance"/>
        public bool RollChance(float probabilityOfReturningTrue) =>
            _randomGeneratorStruct.RollChance(probabilityOfReturningTrue);

        /// <inheritdoc cref="RandomGeneratorStruct.NextDirection2D"/>
        public Vector2 NextDirection2D() => _randomGeneratorStruct.NextDirection2D();

        /// <inheritdoc cref="RandomGeneratorStruct.Next4DirectionInt2D"/>
        public Vector2Int Next4DirectionInt2D() => _randomGeneratorStruct.Next4DirectionInt2D();

        /// <inheritdoc cref="RandomGeneratorStruct.Next8DirectionInt2D"/>
        public Vector2Int Next8DirectionInt2D() => _randomGeneratorStruct.Next8DirectionInt2D();

        /// <inheritdoc cref="RandomGeneratorStruct.Next4Direction2D"/>
        public Vector2 Next4Direction2D() => _randomGeneratorStruct.Next4Direction2D();

        /// <inheritdoc cref="RandomGeneratorStruct.Next8Direction2D"/>
        public Vector2 Next8Direction2D() => _randomGeneratorStruct.Next8Direction2D();

        /// <inheritdoc cref="RandomGeneratorStruct.NextDirection3D"/>
        public Vector3 NextDirection3D() => _randomGeneratorStruct.NextDirection3D();

        #endregion
    }
}
