namespace com.hankcs.hanlp.model.crf.crfpp;


/**
 * @author zhifac
 */
public class KeyValuePair<K,V> : Serializable {

    /**
     * Key of this <code>KeyValuePair</code>.
     */
    private K key;

    /**
     * Gets the key for this pair.
     * @return key for this pair
     */
    public K Key { return key; }

    /**
     * Value of this this <code>KeyValuePair</code>.
     */
    private V value;

    /**
     * Gets the value for this pair.
     * @return value for this pair
     */
    public V Value { return value; }

    /**
     * Creates a new pair
     * @param key The key for this pair
     * @param value The value to use for this pair
     */
    public KeyValuePair(K key, V value) {
        this.key = key;
        this.value = value;
    }

    /**
     * <p><code>string</code> representation of this
     * <code>KeyValuePair</code>.</p>
     *
     * <p>The default name/value delimiter '=' is always used.</p>
     *
     *  @return <code>string</code> representation of this <code>KeyValuePair</code>
     */
    //@Override
    public override string ToString() {
        return key + "=" + value;
    }

    /**
     * <p>Generate a hash code for this <code>KeyValuePair</code>.</p>
     *
     * <p>The hash code is calculated using both the name and
     * the value of the <code>KeyValuePair</code>.</p>
     *
     * @return hash code for this <code>KeyValuePair</code>
     */
    //@Override
    public int GetHashCode() {
        // name's GetHashCode is multiplied by an arbitrary prime number (13)
        // in order to make sure there is a difference in the GetHashCode between
        // these two parameters:
        //  name: a  value: aa
        //  name: aa value: a
        return key.GetHashCode() * 13 + (value == null ? 0 : value.GetHashCode());
    }

    /**
     * <p>Test this <code>KeyValuePair</code> for equality with another
     * <code>Object</code>.</p>
     *
     * <p>If the <code>Object</code> to be tested is not a
     * <code>KeyValuePair</code> or is <code>null</code>, then this method
     * returns <code>false</code>.</p>
     *
     * <p>Two <code>KeyValuePair</code>s are considered equal if and only if
     * both the names and values are equal.</p>
     *
     * @param o the <code>Object</code> to test for
     * equality with this <code>KeyValuePair</code>
     * @return <code>true</code> if the given <code>Object</code> is
     * equal to this <code>KeyValuePair</code> else <code>false</code>
     */
    //@Override
    public bool Equals(Object o) {
        if (this == o) return true;
        if (o is KeyValuePair) {
            KeyValuePair pair = (KeyValuePair) o;
            if (key != null ? !key.Equals(pair.key) : pair.key != null) return false;
            if (value != null ? !value.Equals(pair.value) : pair.value != null) return false;
            return true;
        }
        return false;
    }
}
