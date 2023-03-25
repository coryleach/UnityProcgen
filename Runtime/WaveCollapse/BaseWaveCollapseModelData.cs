using System;
using System.Collections.Generic;

namespace Gameframe.Procgen
{
    [Serializable]
    public class BaseWaveCollapseModelData
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
            public PatternCandidateList[] tiles;
            public int[] this[int tileIndex]
            {
                get => tiles[tileIndex].patterns;
                set => tiles[tileIndex].patterns = value;
            }

            public PropagatorDirection()
            {
            }

            public PropagatorDirection(int tileCount)
            {
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