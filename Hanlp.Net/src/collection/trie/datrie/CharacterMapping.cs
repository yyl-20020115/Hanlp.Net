namespace com.hankcs.hanlp.collection.trie.datrie;

/**
 * 字符映射接口
 */
public interface CharacterMapping
{
    int getInitSize();

    int getCharsetSize();

    int zeroId();

    int[] toIdList(string key);

    int[] toIdList(int codePoint);

    string ToString(int[] ids);
}
