using com.hankcs.hanlp.dictionary.other;
using System.Text;

namespace com.hankcs.hanlp.classification.tokenizers;



public class BigramTokenizer : ITokenizer
{
    public string[] segment(string text)
    {
        if (text.Length == 0) return new string[0];
        char[] charArray = text.ToCharArray();
        CharTable.normalization(charArray);

        // 先拆成字
        List<int[]> atomList = new ();
        int start = 0;
        int end = charArray.Length;
        int offsetAtom = start;
        byte preType = CharType.get(charArray[offsetAtom]);
        byte curType;
        while (++offsetAtom < end)
        {
            curType = CharType.get(charArray[offsetAtom]);
            if (preType == CharType.CT_CHINESE)
            {
                atomList.Add(new int[]{start, offsetAtom - start});
                start = offsetAtom;
            }
            else if (curType != preType)
            {
                // 浮点数识别
                if (charArray[offsetAtom] == '.' && preType == CharType.CT_NUM)
                {
                    while (++offsetAtom < end)
                    {
                        curType = CharType.get(charArray[offsetAtom]);
                        if (curType != CharType.CT_NUM) break;
                    }
                }
                if (preType == CharType.CT_NUM || preType == CharType.CT_LETTER) atomList.Add(new int[]{start, offsetAtom - start});
                start = offsetAtom;
            }
            preType = curType;
        }
        if (offsetAtom == end)
            if (preType == CharType.CT_NUM || preType == CharType.CT_LETTER) atomList.Add(new int[]{start, offsetAtom - start});
        if (atomList.Count==0) return new string[0];
        // 输出
        string[] termArray = new string[atomList.Count - 1];
        IEnumerator<int[]> iterator = atomList.GetEnumerator();
        int[] pre = iterator.next();
        int p = -1;
        while (iterator.MoveNext())
        {
            int[] cur = iterator.next();
            termArray[++p] = new StringBuilder(pre[1] + cur[1]).Append(charArray, pre[0], pre[1]).Append(charArray, cur[0], cur[1]).ToString();
            pre = cur;
        }

        return termArray;
    }

//    public static void main(string args[])
//    {
//        BigramTokenizer bws = new BigramTokenizer();
//        string[] result = bws.segment("@hankcs你好，广阔的世界2０16！\u0000\u0000\t\n\r\n慶祝Coding worlds!");
//        for (string str : result)
//        {
//            Console.WriteLine(str);
//        }
//    }

}
