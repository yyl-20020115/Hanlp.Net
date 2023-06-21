using com.hankcs.hanlp.utility;

namespace com.hankcs.hanlp.model.crf;


[TestClass]
public class CRFModelTest : TestCase
{
//    public void testTemplate() 
//    {
//        FeatureTemplate featureTemplate = FeatureTemplate.create("U05:%x[-2,0]/%x[-1,0]/%x[0,0]");
//        Table table = new Table();
//        table.v = new String[][]{
//            {"那", "S"},
//            {"音", "B"},
//            {"韵", "E"},};
//        char[] parameter = featureTemplate.generateParameter(table, 0);
//        Console.WriteLine(parameter);
//    }

//    public void testTestLoadTemplate() 
//    {
//        Stream _out = new Stream(new FileStream("data/test/_out.bin"));
//        FeatureTemplate featureTemplate = FeatureTemplate.create("U05:%x[-2,0]/%x[-1,0]/%x[0,0]");
//        featureTemplate.save(_out);
//        featureTemplate = new FeatureTemplate();
//        featureTemplate.load(ByteArray.createByteArray("data/test/_out.bin"));
//        Console.WriteLine(featureTemplate);
//    }

//    public void testLoadFromTxt() 
//    {
//        CRFModel model = CRFModel.loadTxt("D:\\Tools\\CRF++-0.58\\example\\seg_cn\\model.txt");
//        Table table = new Table();
//        table.v = new String[][]{
//            {"商", "?"},
//            {"品", "?"},
//            {"和", "?"},
//            {"服", "?"},
//            {"务", "?"},
//        };
//        model.tag(table);
//        Console.WriteLine(table);
//    }

//    public void testSegment() 
//    {
//        HanLP.Config.enableDebug();
//        CRFSegment segment = new CRFSegment();
////        segment.enablePartOfSpeechTagging(true);
//        Console.WriteLine(segment.seg("乐视超级手机能否承载贾布斯的生态梦"));
//    }

    /**
     * 现有的CRF效果不满意，重新制作一份以供训练
     *
     * @
     */
//    public void testPrepareCRFTrainingCorpus() 
//    {
//        TextWriter bw = new TextWriter(new StreamWriter(new FileStream("e:\\2014.txt"), "UTF-8"));
//        CorpusLoader.walk("D:\\Doc\\语料库\\2014_hankcs", new CorpusLoader.Handler()
//                          {
//                              //@Override
//                              public void handle(Document document)
//                              {
//                                  try
//                                  {
//                                      List<List<Word>> sentenceList = document.getSimpleSentenceList();
//                                      if (sentenceList.size() == 0) return;
//                                      for (List<Word> sentence : sentenceList)
//                                      {
//                                          if (sentence.size() == 0) continue;
//                                          for (IWord iWord : sentence)
//                                          {
//                                              String word = iWord.Value;
//                                              String tag = iWord.getLabel();
//                                              String compiledString = compile(tag);
//                                              if (compiledString != null)
//                                              {
//                                                  word = compiledString;
//                                              }
//                                              if (word.Length == 1 || compiledString != null)
//                                              {
//                                                  bw.write(word);
//                                                  bw.write('\t');
//                                                  bw.write('S');
//                                                  bw.write('\n');
//                                              }
//                                              else
//                                              {
//                                                  bw.write(word[0]);
//                                                  bw.write('\t');
//                                                  bw.write('B');
//                                                  bw.write('\n');
//                                                  for (int i = 1; i < word.Length - 1; ++i)
//                                                  {
//                                                      bw.write(word.charAt(i));
//                                                      bw.write('\t');
//                                                      bw.write('M');
//                                                      bw.write('\n');
//                                                  }
//                                                  bw.write(word.charAt(word.Length - 1));
//                                                  bw.write('\t');
//                                                  bw.write('E');
//                                                  bw.write('\n');
//                                              }
//                                          }
//                                          bw.write('\n');
//                                      }
//                                  }
//                                  catch (IOException e)
//                                  {
//                                      e.printStackTrace();
//                                  }
//                              }
//                          }
//
//        );
//        bw.Close();
//    }

//    public void testEnglishAndNumber() 
//    {
//        String text = "2.34米";
////        Console.WriteLine(CRFSegment.atomSegment(text.ToCharArray()));
//        HanLP.Config.enableDebug();
//        CRFSegment segment = new CRFSegment();
//        Console.WriteLine(segment.seg(text));
//    }

    public static String compile(String tag)
    {
        if (tag.StartsWith("m")) return "M";
        else if (tag.Equals("x")) return "W";
        else if (tag.Equals("nx")) return "W";
        return null;
    }

    public void testLoadModelWithBiGramFeature() 
    {
        String path = HanLP.Config.CRFSegmentModelPath + Predefine.BIN_EXT;
        CRFModel model = new CRFModel(new BinTrie<FeatureFunction>());
        model.load(ByteArray.createByteArray(path));

        Table table = new Table();
        String text = "人民生活进一步改善了";
        table.v = new String[text.Length][2];
        for (int i = 0; i < text.Length; i++)
        {
            table.v[i][0] = String.valueOf(text.charAt(i));
        }

        model.tag(table);
//        Console.WriteLine(table);
    }

//    public void testRemoveSpace() 
//    {
//        String inputPath = "E:\\2014.txt";
//        String outputPath = "E:\\2014f.txt";
//        TextReader br = IOUtil.newBufferedReader(inputPath);
//        TextWriter bw = IOUtil.newBufferedWriter(outputPath);
//        String line = "";
//        int preLength = 0;
//        while ((line = br.ReadLine()) != null)
//        {
//            if (preLength == 0 && line.Length == 0) continue;
//            bw.write(line);
//            bw.newLine();
//            preLength = line.Length;
//        }
//        bw.Close();
//    }
}