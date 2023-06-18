namespace com.hankcs.hanlp.recognition.nt;


[TestClass]

public class OrganizationRecognitionTest : TestCase
{
//    public void testSeg() 
//    {
//        HanLP.Config.enableDebug();
//        DijkstraSegment segment = new DijkstraSegment();
//        segment.enableCustomDictionary(false);
//
//        segment.enableOrganizationRecognize(true);
//        Console.WriteLine(segment.seg("东欧的球队"));
//    }
//
//    public void testGeneratePatternJavaCode() 
//    {
//        CommonStringDictionary commonStringDictionary = new CommonStringDictionary();
//        commonStringDictionary.load("data/dictionary/organization/nt.pattern.txt");
//        StringBuilder sb = new StringBuilder();
//        HashSet<String> keySet = commonStringDictionary.Keys;
//        CommonStringDictionary secondDictionary = new CommonStringDictionary();
//        secondDictionary.load("data/dictionary/organization/outerNT.pattern.txt");
//        keySet.addAll(secondDictionary.Keys);
//        for (String pattern : keySet)
//        {
//            sb.append("trie.addKeyword(\"" + pattern + "\");\n");
//        }
//        IOUtil.saveTxt("data/dictionary/organization/code.txt", sb.ToString());
//    }
//
//    public void testRemoveP() 
//    {
//        DictionaryMaker maker = DictionaryMaker.load(HanLP.Config.OrganizationDictionaryPath);
//        for (Map.Entry<String, Item> entry : maker.entrySet())
//        {
//            String word = entry.getKey();
//            Item item = entry.getValue();
//            CoreDictionary.Attribute attribute = LexiconUtility.getAttribute(word);
//            if (attribute == null) continue;
//            if (item.containsLabel("P") && attribute.hasNatureStartsWith("u"))
//            {
//                Console.WriteLine(item + "\t" + attribute);
//                item.removeLabel("P");
//            }
//        }
//        maker.saveTxtTo(HanLP.Config.OrganizationDictionaryPath);
//    }
}