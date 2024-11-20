using Stasok.Utils;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Stasok.LangSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LangSystem : UdonSharpBehaviour
    {
        // !!! Вешать его на "LangSetting.GetPathLangSystem()" !!!

        [Header("индекс языка при старте ([0:ru],[1:en],[2:ja])")]
        [SerializeField] int defaultLang = 0;
        public int CurrentLang { get => _currentLang; }
        private int _currentLang;

        [TextArea]
        [Header("игрок/его язык")]
        [SerializeField] string ignorePlayers = "stasok/0";

        private UdonSharpBehaviour[] _langReceivers = new UdonSharpBehaviour[700];
        private string[][] _lettersLang =  {
        new string[] { // русский
// высокий регистр
"А",
"Б",
"В",
"Г",
"Д",
"Е",
"Ё",
"Ж",
"З",
"И",
"Й",
"К",
"Л",
"М",
"Н",
"О",
"П",
"Р",
"С",
"Т",
"У",
"Ф",
"Х",
"Ц",
"Ч",
"Ш",
"Щ",
"Ъ",
"Ы",
"Ь",
"Э",
"Ю",
"Я",

// нижний регистр
"а",
"б",
"в",
"г",
"д",
"е",
"ё",
"ж",
"з",
"и",
"й",
"к",
"л",
"м",
"н",
"о",
"п",
"р",
"с",
"т",
"у",
"ф",
"х",
"ц",
"ч",
"ш",
"щ",
"ъ",
"ы",
"ь",
"э",
"ю",
"я",
        },
        new string[] { // английский
// высокий регистр
"A",
"B",
"C",
"D",
"E",
"F",
"G",
"H",
"I",
"J",
"K",
"L",
"M",
"N",
"O",
"P",
"Q",
"R",
"S",
"T",
"U",
"V",
"W",
"X",
"Y",
"Z",

// нижний регистр
"a",
"b",
"c",
"d",
"e",
"f",
"g",
"h",
"i",
"j",
"k",
"l",
"m",
"n",
"o",
"p",
"q",
"r",
"s",
"t",
"u",
"v",
"w",
"x",
"y",
"z",
        },
        new string[] { // японский
// Хирагана
"ぁ",
"あ",
"ぃ",
"い",
"ぅ",
"う",
"ぇ",
"え",
"ぉ",
"お",
"か",
"が",
"き",
"ぎ",
"く",
"ぐ",
"け",
"げ",
"こ",
"ご",
"さ",
"ざ",
"し",
"じ",
"す",
"ず",
"せ",
"ぜ",
"そ",
"ぞ",
"た",
"だ",
"ち",
"ぢ",
"っ",
"つ",
"づ",
"て",
"で",
"と",
"ど",
"な",
"に",
"ぬ",
"ね",
"の",
"は",
"ば",
"ぱ",
"ひ",
"び",
"ぴ",
"ふ",
"ぶ",
"ぷ",
"へ",
"べ",
"ぺ",
"ほ",
"ぼ",
"ぽ",
"ま",
"み",
"む",
"め",
"も",
"ゃ",
"や",
"ゅ",
"ゆ",
"ょ",
"よ",
"ら",
"り",
"る",
"れ",
"ろ",
"ゎ",
"わ",
"ゐ",
"ゑ",
"を",
"ん",
"ゔ",
"ゕ",
"ゖ",
"゙",
"゚",
"゛",
"゜",
"ゝ",
"ゞ",
"ゟ",


// Катакана 
"゠",
"ァ",
"ア",
"ィ",
"イ",
"ゥ",
"ウ",
"ェ",
"エ",
"ォ",
"オ",
"カ",
"ガ",
"キ",
"ギ",
"ク",
"グ",
"ケ",
"ゲ",
"コ",
"ゴ",
"サ",
"ザ",
"シ",
"ジ",
"ス",
"ズ",
"セ",
"ゼ",
"ソ",
"ゾ",
"タ",
"ダ",
"チ",
"ヂ",
"ッ",
"ツ",
"ヅ",
"テ",
"デ",
"ト",
"ド",
"ナ",
"ニ",
"ヌ",
"ネ",
"ノ",
"ハ",
"バ",
"パ",
"ヒ",
"ビ",
"ピ",
"フ",
"ブ",
"プ",
"ヘ",
"ベ",
"ペ",
"ホ",
"ボ",
"ポ",
"マ",
"ミ",
"ム",
"メ",
"モ",
"ャ",
"ヤ",
"ュ",
"ユ",
"ョ",
"ヨ",
"ラ",
"リ",
"ル",
"レ",
"ロ",
"ヮ",
"ワ",
"ヰ",
"ヱ",
"ヲ",
"ン",
"ヴ",
"ヵ",
"ヶ",
"ヷ",
"ヸ",
"ヹ",
"ヺ",
"・",
"ー",
"ヽ",
"ヾ",
"ヿ",
        },
        new string[] { // китайский
"诶",
"比",
"西",
"迪",
"伊",
"艾弗",
"吉",
"艾尺",
"艾",
"杰",
"开",
"艾勒",
"艾马",
"艾娜",
"哦",
"屁",
"吉吾",
"艾儿",
"艾丝",
"提",
"伊吾",
"维",
"豆贝尔维",
"艾克斯",
"吾艾",
"贼德",
        },
        new string[] { // немецкий
// высокий регистр
"Ä",
"Ö",
"ß",
"Ü",
// нижний регистр
"ä",
"ö",
"ü",
        },
    };

        public bool ChangeLang { get => _changeLang; }
        private bool _changeLang;

        public int RecipientsCount { get => _recipientsCount; }
        private int _recipientsCount;

        public int LangableCount { get => _langableCount; }
        private int _langableCount;
        

        private void Start()
        {
            CheckUserLang();
        }

        private void CheckUserLang()
        {
            foreach (var IgnorePlayer in ignorePlayers.Split('\n'))
            {
                if (IgnorePlayer.Contains(Networking.LocalPlayer.displayName))
                {
                    string lang = IgnorePlayer.Split('/')[1];
                    ToLang(System.Convert.ToInt16(lang));
                    return;
                }
            }

            for (int iLang = 0; iLang < _lettersLang.Length; iLang++)
            {
                for (int iWord = 0; iWord < _lettersLang[iLang].Length; iWord++)
                {
                    foreach (var langSymbol in Networking.LocalPlayer.displayName)
                    {
                        if (langSymbol.ToString() == _lettersLang[iLang][iWord])
                        {
                            ToLang(iLang);
                            return;
                        }
                    }
                }
            }

            ToLang(defaultLang);
        }

        public void AddLangReceiver(UdonSharpBehaviour udon)
        {
            if (_recipientsCount >= _langReceivers.Length)
                ArrayUtils.Resize(ref _langReceivers, _langReceivers.Length + 100);

            _langReceivers[_recipientsCount++] = udon;
        }

        public void SetRU() { ToLang(0); }
        public void SetEN() { ToLang(1); }
        public void SetJA() { ToLang(2); }

        private void ToLang(int lang)
        {
            if (lang == CurrentLang) return;

            _currentLang = lang;
            _langableCount = 0;
            _changeLang = true;
        }

        private void Update()
        {
            if (!_changeLang) return;

            if (_langableCount < _recipientsCount)
                _langReceivers[_langableCount++].SendCustomEvent("ChangeLang");
            else
                _changeLang = false;
        }

#if UNITY_EDITOR && !COMPILER_UDONSHARP
        [CustomEditor(typeof(LangSystem))]
        public class LangSystemInspector : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                LangSystem script = (LangSystem)target;
                if (GUILayout.Button(nameof(SetRU)))
                    script.SetRU();
                if (GUILayout.Button(nameof(SetEN)))
                    script.SetEN();
                if (GUILayout.Button(nameof(SetJA)))
                    script.SetJA();
            }
        }
#endif
    }
}
