namespace com.hankcs.hanlp.model.trigram;


[TestClass]

public class CharacterBasedGenerativeModelTest : TestCase
{
//    public void testTrainAndSegment() 
//    {
//        CharacterBasedGenerativeModel model = new CharacterBasedGenerativeModel();
//        CorpusLoader.walk("D:\\JavaProjects\\HanLP\\data\\test\\cbgm", new CorpusLoader.Handler()
//        {
//            //@Override
//            public void handle(Document document)
//            {
//                for (List<Word> sentence : document.getSimpleSentenceList())
//                {
//                    model.learn(sentence);
//                }
//            }
//        });
//        model.train();
////        Stream Out = new Stream(new FileStream(HanLP.Config.HMMSegmentModelPath));
////        model.save(Out);
////        Out.Close();
////        model.load(ByteArray.createByteArray(HanLP.Config.HMMSegmentModelPath));
//        String text = "中国领土";
//        char[] charArray = text.ToCharArray();
//        char[] tag = model.tag(charArray);
//        Console.WriteLine(tag);
//    }
//
//    public void testLoad() 
//    {
//        CharacterBasedGenerativeModel model = new CharacterBasedGenerativeModel();
//        model.load(ByteArray.createByteArray(HanLP.Config.HMMSegmentModelPath));
//        String text = "我实现了一个基于char Based TriGram的分词器";
//        char[] sentence = text.ToCharArray();
//        char[] tag = model.tag(sentence);
//
//        List<String> termList = new LinkedList<String>();
//        int offset = 0;
//        for (int i = 0; i < tag.Length; offset += 1, ++i)
//        {
//            switch (tag[i])
//            {
//                case 'b':
//                {
//                    int begin = offset;
//                    while (tag[i] != 'e')
//                    {
//                        offset += 1;
//                        ++i;
//                        if (i == tag.Length)
//                        {
//                            break;
//                        }
//                    }
//                    if (i == tag.Length)
//                    {
//                        termList.Add(new String(sentence, begin, offset - begin));
//                    }
//                    else
//                        termList.Add(new String(sentence, begin, offset - begin + 1));
//                }
//                break;
//                default:
//                {
//                    termList.Add(new String(sentence, offset, 1));
//                }
//                break;
//            }
//        }
//        Console.WriteLine(tag);
//        Console.WriteLine(termList);
//    }
//
//    public void testSegment() 
//    {
//        HanLP.Config.ShowTermNature = false;
//        String text = "我实现了一个基于char Based TriGram的分词器";
//        Segment segment = new HMMSegment();
//        List<Term> termList = segment.seg(text);
//        Console.WriteLine(termList);
//    }
}