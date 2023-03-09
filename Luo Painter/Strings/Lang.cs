using System.Collections;
using System.Collections.Generic;

namespace Luo_Painter.Strings
{
    public sealed class Languages : IEnumerable<string>
    {
        public int Count => 13;

        public int this[string value]
        {
            get
            {
                switch (value)
                {
                    case "ar": return 1;
                    case "de": return 2;
                    case "en-US": return 3;
                    case "es": return 4;
                    case "fr": return 5;
                    case "it": return 6;
                    case "ja": return 7;
                    case "ko": return 8;
                    case "nl": return 9;
                    case "pt": return 10;
                    case "ru": return 11;
                    case "zh-CN": return 12;
                    default: return 0;
                }
            }
        }
        public string this[int index]
        {
            get
            {
                switch (index)
                {
                    case 1: return "ar";
                    case 2: return "de";
                    case 3: return "en-US";
                    case 4: return "es";
                    case 5: return "fr";
                    case 6: return "it";
                    case 7: return "ja";
                    case 8: return "ko";
                    case 9: return "nl";
                    case 10: return "pt";
                    case 11: return "ru";
                    case 12: return "zh-CN";
                    default: return string.Empty;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<string> GetEnumerator()
        {
            for (int i = 0; i < 13; i++)
            {
                yield return this[i];
            }
        }

    }
}