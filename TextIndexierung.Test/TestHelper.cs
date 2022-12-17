using FluentAssertions;

namespace TextIndexierung.Test
{
    internal class TestHelper
    {
        internal static void AssertLectureLcpArray(int[] lcpArray)
        {
            lcpArray.Should().Equal(0, 0, 1, 2, 2, 5, 0, 2, 1, 1, 4, 0, 3);
        }
    }
}
