/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
namespace com.hankcs.hanlp.collection.dartsclone;

/**
 * 模拟C++中的pair，也兼容JavaFX中的KeyValuePair
 * @author manabe
 */
public class KeyValuePair<T, U>
{
    public T first;
    public U second;

    public KeyValuePair(T first, U second)
    {
        this.first = first;
        this.second = second;
    }

    public T getFirst()
    {
        return first;
    }

    public T Key()
    {
        return first;
    }

    public U getSecond()
    {
        return second;
    }

    public U Value()
    {
        return second;
    }

    //@Override
    public override string ToString()
    {
        return first + "=" + second;
    }
}
