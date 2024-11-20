
namespace Stasok.Utils
{
    public static class MathUtils
    {
        public static float Convert(double value, double From1, double From2, double To1, double To2)
        {
            return (float)((value - From1) / (From2 - From1) * (To2 - To1) + To1);
        }
    }
}
