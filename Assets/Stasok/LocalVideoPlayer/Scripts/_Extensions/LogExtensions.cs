using UnityEngine;

public static class Logger
{
    public static void Log(string methodName, string description)
    {
        Debug.Log($"<{methodName}> [{description}]\n");
    }

    public static void LogColorInfo(string name, Color color)
    {
        Debug.Log($"Color Info Name: {name}\n" +
            $"R: {color.r}\n" +
            $"G: {color.g}\n" +
            $"B: {color.b}\n" +
            $"A: {color.a}\n" +
            $"gamma: {color.gamma}\n" +
            $"grayscale: {color.grayscale}\n" +
            $"linear: {color.linear}\n" +
            $"maxColorComponent: {color.maxColorComponent}\n"
#if !COMPILER_UDONSHARP
            +
            $"RGB Hex: {ColorUtility.ToHtmlStringRGB(color)}\n" +
            $"RGBA Hex: {ColorUtility.ToHtmlStringRGBA(color)}\n"
#endif
        );
    }
}
