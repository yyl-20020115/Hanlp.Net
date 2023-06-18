/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
namespace com.hankcs.hanlp.collection.dartsclone;

/**
 * 模拟C++中的pair，也兼容JavaFX中的Pair
 * @author manabe
 */
public class Pair<T, U>
{
    public T first;
    public U second;

    public Pair(T first, U second)
    {
        this.first = first;
        this.second = second;
    }

    public T getFirst()
    {
        return first;
    }

    public T getKey()
    {
        return first;
    }

    public U getSecond()
    {
        return second;
    }

    public U getValue()
    {
        return second;
    }

    //@Override
    public override string ToString()
    {
        return first + "=" + second;
    }
}
