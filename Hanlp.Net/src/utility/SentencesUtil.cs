using com.hankcs.hanlp.corpus.tag;
using com.hankcs.hanlp.seg.common;
using System.Text;

namespace com.hankcs.hanlp.utility;



/**
 * 文本断句
 */
public class SentencesUtil
{
    /**
     * 将文本切割为最细小的句子（逗号也视作分隔符）
     *
     * @param content
     * @return
     */
    public static List<string> ToSentenceList(string content) => toSentenceList(content.ToCharArray(), true);

    /**
     * 文本分句
     *
     * @param content  文本
     * @param shortest 是否切割为最细的单位（将逗号也视作分隔符）
     * @return
     */
    public static List<string> toSentenceList(string content, bool shortest)
    {
        return toSentenceList(content.ToCharArray(), shortest);
    }

    public static List<string> toSentenceList(char[] chars)
    {
        return toSentenceList(chars, true);
    }

    public static List<string> toSentenceList(char[] chars, bool shortest)
    {

        StringBuilder sb = new StringBuilder();

        List<string> sentences = new ();

        for (int i = 0; i < chars.Length; ++i)
        {
            if (sb.Length == 0 && (char.IsWhiteSpace(chars[i]) || chars[i] == ' '))
            {
                continue;
            }

            sb.Append(chars[i]);
            if (chars[i] == '；' && !shortest) continue;
            switch (chars[i])
            {
                case '.':
                    if (i < chars.Length - 1 && chars[i + 1] > 128)
                    {
                        insertIntoList(sb, sentences);
                        sb = new StringBuilder();
                    }
                    break;
                case '…':
                {
                    if (i < chars.Length - 1 && chars[i + 1] == '…')
                    {
                        sb.Append('…');
                        ++i;
                        insertIntoList(sb, sentences);
                        sb = new StringBuilder();
                    }
                }
                break;
                case '，':
                case ',':
                case ';':
                case '；':
                case ' ':
                case '	':
                case ' ':
                case '。':
                case '!':
                case '！':
                case '?':
                case '？':
                case '\n':
                case '\r':
                    insertIntoList(sb, sentences);
                    sb = new StringBuilder();
                    break;
            }
        }

        if (sb.Length > 0)
        {
            insertIntoList(sb, sentences);
        }

        return sentences;
    }

    private static void insertIntoList(StringBuilder sb, List<string> sentences)
    {
        string content = sb.ToString().Trim();
        if (content.Length > 0)
        {
            sentences.Add(content);
        }
    }

    /**
     * 句子中是否含有词性
     *
     * @param sentence
     * @param nature
     * @return
     */
    public static bool hasNature(List<Term> sentence, Nature nature)
    {
        foreach (Term term in sentence)
        {
            if (term.nature == nature)
            {
                return true;
            }
        }

        return false;
    }
}
