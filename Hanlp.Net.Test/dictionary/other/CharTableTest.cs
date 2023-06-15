namespace com.hankcs.hanlp.dictionary.other;


[TestClass]
public class CharTableTest : TestCase
{
    [TestMethod]
    public void TestNormalization() 
    {
        Console.WriteLine(CharTable.convert('？'));
        AssertEquals('(', CharTable.convert('（'));
    }
    [TestMethod]
    public void TestNormalizeSpace() {
        AssertEquals(CharTable.convert('\t'),' ');
        AssertEquals(CharTable.convert('\n'),' ');
        AssertEquals(CharTable.convert('\f'),' ');
    }
//    public void testConvert() 
//    {
//        Console.WriteLine(CharTable.CONVERT['關']);
//        Console.WriteLine(CharTable.CONVERT['Ａ']);
//        Console.WriteLine(CharTable.CONVERT['“']);
//        Console.WriteLine(CharTable.CONVERT['．']);
//    }
//
//    public void testEnd() 
//    {
//        Console.WriteLine(CharTable.CONVERT['，']);
//        Console.WriteLine(CharTable.CONVERT['。']);
//        Console.WriteLine(CharTable.CONVERT['！']);
//        Console.WriteLine(CharTable.CONVERT['…']);
//    }
//
//    public void testFix() 
//    {
//        char[] CONVERT = CharTable.CONVERT;
//        CONVERT['.'] = '.';
//        CONVERT['．'] = '.';
//        CONVERT['。'] = '.';
//        CONVERT['！'] = '!';
//        CONVERT['，'] = ',';
//        CONVERT['!'] = '!';
//        CONVERT['#'] = '#';
//        CONVERT['&'] = '&';
//        CONVERT['*'] = '*';
//        CONVERT[','] = ',';
//        CONVERT['/'] = '/';
//        CONVERT[';'] = ';';
//        CONVERT['?'] = '?';
//        CONVERT['\\'] = '\\';
//        CONVERT['^'] = '^';
//        CONVERT['_'] = '_';
//        CONVERT['`'] = '`';
//        CONVERT['|'] = '|';
//        CONVERT['~'] = '~';
//        CONVERT['¡'] = '¡';
//        CONVERT['¦'] = '¦';
//        CONVERT['´'] = '´';
//        CONVERT['¸'] = '¸';
//        CONVERT['¿'] = '¿';
//        CONVERT['ˇ'] = 'ˇ';
//        CONVERT['ˉ'] = 'ˉ';
//        CONVERT['ˊ'] = 'ˊ';
//        CONVERT['ˋ'] = 'ˋ';
//        CONVERT['˜'] = '˜';
//        CONVERT['—'] = '—';
//        CONVERT['―'] = '―';
//        CONVERT['‖'] = '‖';
//        CONVERT['…'] = '…';
//        CONVERT['∕'] = '∕';
//        CONVERT['︳'] = '︳';
//        CONVERT['︴'] = '︴';
//        CONVERT['﹉'] = '﹉';
//        CONVERT['﹊'] = '﹊';
//        CONVERT['﹋'] = '﹋';
//        CONVERT['﹌'] = '﹌';
//        CONVERT['﹍'] = '﹍';
//        CONVERT['﹎'] = '﹎';
//        CONVERT['﹏'] = '﹏';
//        CONVERT['﹐'] = '﹐';
//        CONVERT['﹑'] = '﹑';
//        CONVERT['﹔'] = '﹔';
//        CONVERT['﹖'] = '﹖';
//        CONVERT['﹟'] = '﹟';
//        CONVERT['﹠'] = '﹠';
//        CONVERT['﹡'] = '﹡';
//        CONVERT['﹨'] = '﹨';
//        CONVERT['＇'] = '＇';
//        CONVERT['；'] = '；';
//        CONVERT['？'] = '？';
//        CONVERT['幣'] = '币';
//        CONVERT['繫'] = '系';
//        CONVERT['眾'] = '众';
//        CONVERT['龕'] = '龛';
//        CONVERT['製'] = '制';
//        for (int i = 0; i < CONVERT.Length; i++)
//        {
//            if (CONVERT[i] == '\u0000')
//            {
//                if (i != '\u0000') CONVERT[i] = (char) i;
//                else CONVERT[i] = ' ';
//            }
//        }
//        ObjectOutputStream _out = new ObjectOutputStream(new FileOutputStream(HanLP.Config.CharTablePath));
//        _out.writeObject(CONVERT);
//        _out.close();
//    }
//
//    public void testImportSingleCharFromTraditionalChineseDictionary() 
//    {
////        char[] CONVERT = CharTable.CONVERT;
////        StringDictionary dictionary = new StringDictionary("=");
////        dictionary.load(HanLP.Config.t2sDictionaryPath);
////        for (Map.Entry<String, String> entry : dictionary.entrySet())
////        {
////            String key = entry.getKey();
////            if (key.Length() != 1) continue;
////            String value = entry.getValue();
////            char t = key.charAt(0);
////            char s = value.charAt(0);
//////            if (CONVERT[t] != s)
//////            {
//////                Console.printf("%s\t%c=%c\n", entry, t, CONVERT[t]);
//////            }
////            CONVERT[t] = s;
////        }
////
////        ObjectOutputStream _out = new ObjectOutputStream(new FileOutputStream(HanLP.Config.CharTablePath));
////        _out.writeObject(CONVERT);
////        _out.close();
//    }
//
//    public void testDumpCharTable() 
//    {
//        BufferedWriter bw = IOUtil.newBufferedWriter(HanLP.Config.CharTablePath.replace(".bin.yes", ".txt"));
//        char[] CONVERT = CharTable.CONVERT;
//        for (int i = 0; i < CONVERT.Length; i++)
//        {
//            if (i != CONVERT[i])
//            {
//                bw.write(String.format("%c=%c\n", i, CONVERT[i]));
//            }
//        }
//        bw.close();
//    }
//
//    public void testLoadCharTableFromTxt() 
//    {
////        CharTable.load(HanLP.Config.CharTablePath.replace(".bin.yes", ".txt"));
//    }
}
