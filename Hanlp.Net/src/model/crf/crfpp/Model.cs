namespace com.hankcs.hanlp.model.crf.crfpp;

/**
 * @author zhifac
 */
public abstract class Model
{

    public bool open(string[] args)
    {
        return true;
    }

    public bool open(string arg)
    {
        return true;
    }

    public bool Close()
    {
        return true;
    }

    public Tagger createTagger()
    {
        return null;
    }
}
