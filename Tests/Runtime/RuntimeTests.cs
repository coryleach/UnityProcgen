using System;
using NUnit.Framework;
using UnityEngine;

namespace Gameframe.Procgen.Tests.Runtime
{
    public class RuntimeTests
    {
        [Test]
        public void NoiseRNG_IntRange()
        {
            var noiseRng = new NoiseRng((uint)DateTime.UtcNow.Ticks);

            const int maxRange = 2;
            var totals = new int[maxRange+1];

            for (var i = 0; i < totals.Length; i++)
            {
                totals[i] = 0;
            }

            const int count = 200;
            for (var i = 0; i < count; i++)
            {
                var next = noiseRng.NextIntRange(0, maxRange);
                totals[next] += 1;
            }

            for (var i = 0; i < totals.Length; i++)
            {
                Debug.Log($"{i} : {totals[i]} / {count} = {(totals[i] / (float)count)}");
            }
        }

        [Test]
        public void NoiseRNG_RollChance_33percent()
        {
            var noiseRng = new NoiseRng((uint)DateTime.UtcNow.Ticks);

            const float probability = 0.33f;
            var totals = new int[2];

            for (var i = 0; i < totals.Length; i++)
            {
                totals[i] = 0;
            }

            const int count = 200;
            for (var i = 0; i < count; i++)
            {
                var next = noiseRng.RollChance(probability);
                if (next)
                {
                    totals[1] += 1;
                }
                else
                {
                    totals[0] += 1;
                }
            }

            for (var i = 0; i < totals.Length; i++)
            {
                Debug.Log($"{(i == 1 ? "true" : "false")} : {totals[i]} / {count} = {(totals[i] / (float)count)}");
            }
        }
    }
}
