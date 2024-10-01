
using UnityEngine;

namespace Stasok.Extensions
{
    public static class TransformExtensions
    {
        public static void CopyPosAndRot(this Transform src, Transform target)
        {
            src.position = target.position;
            src.rotation = target.rotation;
        }
    }
}