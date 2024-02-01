namespace com.hankcs.hanlp.utility;

/**
 * 字符集识别辅助工具类
 */
public class CharacterHelper
{

    public static bool isSpaceLetter(char input)
    {
        return input == 8 || input == 9
                || input == 10 || input == 13
                || input == 32 || input == 160;
    }

    public static bool isEnglishLetter(char input)
    {
        return (input >= 'a' && input <= 'z')
                || (input >= 'A' && input <= 'Z');
    }

    public static bool isArabicNumber(char input)
    {
        return input >= '0' && input <= '9';
    }

    public static bool isCJKCharacter(char input)
    {
        //TODO:fix
#if false
        char.UnicodeBlock ub = char.UnicodeBlock.of(input);
        if (ub == char.UnicodeBlock.CJK_UNIFIED_IDEOGRAPHS
                || ub == char.UnicodeBlock.CJK_COMPATIBILITY_IDEOGRAPHS
                || ub == char.UnicodeBlock.CJK_UNIFIED_IDEOGRAPHS_EXTENSION_A
                //全角数字字符和日韩字符
                || ub == char.UnicodeBlock.HALFWIDTH_AND_FULLWIDTH_FORMS
                //韩文字符集
                || ub == char.UnicodeBlock.HANGUL_SYLLABLES
                || ub == char.UnicodeBlock.HANGUL_JAMO
                || ub == char.UnicodeBlock.HANGUL_COMPATIBILITY_JAMO
                //日文字符集
                || ub == char.UnicodeBlock.HIRAGANA //平假名
                || ub == char.UnicodeBlock.KATAKANA //片假名
                || ub == char.UnicodeBlock.KATAKANA_PHONETIC_EXTENSIONS
                )
        {
            return true;
        }
        else
        {
            return false;
        }
#endif
        return false;
        //其他的CJK标点符号，可以不做处理
        //|| ub == char.UnicodeBlock.CJK_SYMBOLS_AND_PUNCTUATION
        //|| ub == char.UnicodeBlock.GENERAL_PUNCTUATION
    }


    /**
     * 进行字符规格化（全角转半角，大写转小写处理）
     *
     * @param input
     * @return char
     */
    public static char regularize(char input)
    {
        if (input == 12288)
        {
            input = (char) 32;

        }
        else if (input > 65280 && input < 65375)
        {
            input = (char) (input - 65248);

        }
        else if (input >= 'A' && input <= 'Z')
        {
            input = (char)(input + 32);
        }

        return input;
    }

}
