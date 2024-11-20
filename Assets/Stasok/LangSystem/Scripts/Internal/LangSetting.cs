using UnityEngine;

namespace Stasok.LangSystem
{
    public static class LangSetting
    {
        public static string GetPathLangSystem()
        {
            return "Global/LangSystem";
        }

        public static LangSystem GetLangSystem()
        {
            return GameObject.Find(GetPathLangSystem()).GetComponent<LangSystem>();
        }
    }
}
