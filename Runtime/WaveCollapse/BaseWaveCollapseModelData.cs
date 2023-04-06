using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameframe.Procgen
{
    /// <summary>
    /// Base class for the data required to run a WaveCollapseModel
    /// Contains the list of patterns, the weights associated with each pattern and the size of each pattern
    /// Propagator contains data about which how patterns match and fit together which is used while collapsing the state
    /// </summary>
    public abstract class BaseWaveCollapseModelData
    {
        public List<PatternList> patterns;
        public double[] weights;
        public int patternSize;
        public Propagator propagator;

        [Serializable]
        public class PatternList
        {
            public int[] array;

            public PatternList()
            {
            }

            public PatternList(int[] patterns)
            {
                this.array = patterns;
            }

            public int this[int i]
            {
                get => array[i];
                set => array[i] = value;
            }

            public int Length => array.Length;
        }

        /// <summary>
        /// Propagator class contains the data about the relationship between patterns and how they fit together in each direction
        /// </summary>
        [Serializable]
        public class Propagator
        {
            public PropagatorDirection[] directions;
            public PropagatorDirection this[int direction]
            {
                get => directions[direction];
                set => directions[direction] = value;
            }

            public Propagator()
            {
                directions = new PropagatorDirection[4];
            }
        }

        [Serializable]
        public class PropagatorDirection
        {
            public int dx;
            public int dy;
            public PatternCandidateList[] tiles;
            public int[] this[int tileIndex]
            {
                get => tiles[tileIndex].patterns;
                set => tiles[tileIndex].patterns = value;
            }

            public Vector2Int Dir => new Vector2Int(dx, dy);

            public PropagatorDirection()
            {
            }

            public PropagatorDirection(int tileCount, int dx, int dy)
            {
                this.dx = dx;
                this.dy = dy;
                tiles = new PatternCandidateList[tileCount];
                for (int i = 0; i < tiles.Length; i++)
                {
                    tiles[i] = new PatternCandidateList();
                }
            }
        }

        [Serializable]
        public class PatternCandidateList
        {
            public int[] patterns;
        }
    }
}
