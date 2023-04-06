using NUnit.Framework;

namespace Gameframe.Procgen.Tests.Editor
{
    public class EditorTests
    {
        [Test]
        public void Agrees_N3_dx_1()
        {
            int size = 3;

            int[] p1 =
            {
                1, 2, 3,
                4, 5, 6,
                7, 8, 9
            };

            int[] p2 =
            {
                2, 3, 0,
                5, 6, 0,
                8, 9, 0
            };

            Assert.IsTrue(WaveCollapseExtensions.Agrees(p1, p2, 1, 0, size));
        }

        [Test]
        public void Agrees_N4_dx_1()
        {
            int size = 4;

            int[] p1 =
            {
                01, 02, 03, 04,
                05, 06, 07, 08,
                09, 10, 11, 12,
                13, 14, 15, 16,
            };

            int[] p2 =
            {
                03, 04, 00, 00,
                07, 08, 00, 00,
                11, 12, 00, 00,
                15, 16, 00, 00,
            };

            int[] p3 =
            {
                02, 03, 04, 00,
                06, 07, 08, 00,
                10, 11, 12, 00,
                14, 15, 16, 00,
            };

            Assert.IsTrue(WaveCollapseExtensions.Agrees(p1, p3, 1, 0, size));
        }

        [Test]
        public void Agrees_2()
        {
            int size = 3;

            int[] p1 =
            {
                1, 2, 3,
                4, 5, 6,
                7, 8, 9
            };

            int[] p2 =
            {
                0, 1, 2,
                0, 4, 5,
                0, 7, 8
            };

            Assert.IsTrue(WaveCollapseExtensions.Agrees(p1, p2, -1, 0, size));
        }

        [Test]
        public void Agrees_3()
        {
            int size = 3;

            int[] p1 =
            {
                1, 2, 3,
                4, 5, 6,
                7, 8, 9
            };

            int[] p2 =
            {
                0, 0, 0, //y = 0
                1, 2, 3, //y = 1
                4, 5, 6  //y = 2 y is inverted here
            };

            Assert.IsTrue(WaveCollapseExtensions.Agrees(p1, p2, 0, -1, size));
        }

        [Test]
        public void Agrees_4()
        {
            int size = 3;

            int[] p1 =
            {
                0, 1, 2,
                3, 4, 5,
                6, 7, 8
            };

            int[] p2 =
            {
                3, 4, 5, //y = 0
                6, 7, 8, //y = 1
                0, 0, 0  //y = 2 y is inverted here
            };

            Assert.IsTrue(WaveCollapseExtensions.Agrees(p1, p2, 0, 1, size));
        }

        [Test]
        public void Agrees_5()
        {
            int size = 3;

            int[] p1 =
            {
                0, 0, 0, // y = 0
                1, 1, 1, // y = 1
                2, 2, 2  // y = 2
            };

            int[] p2 =
            {
                0, 0, 0, //y = 0
                1, 1, 1, //y = 1
                1, 1, 1  //y = 2 y is inverted here
            };

            Assert.IsTrue(WaveCollapseExtensions.Agrees(p1, p2, 0, 1, size));
        }
    }
}
