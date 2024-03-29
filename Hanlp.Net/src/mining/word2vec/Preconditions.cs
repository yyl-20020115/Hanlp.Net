using System.Text;

namespace com.hankcs.hanlp.mining.word2vec;

class Preconditions
{

    private Preconditions()
    {
    }

    /**
     * Ensures the truth of an expression involving one or more parameters to the calling method.
     *
     * @param expression a bool expression
     * @ if {@code expression} is false
     */
    public static void checkArgument(bool expression)
    {
        if (!expression)
        {
            throw new ArgumentException();
        }
    }

    /**
     * Ensures the truth of an expression involving one or more parameters to the calling method.
     *
     * @param expression   a bool expression
     * @param errorMessage the exception message to use if the check fails; will be converted to a
     *                     string using {@link string#valueOf(Object)}
     * @ if {@code expression} is false
     */
    public static void checkArgument(bool expression, Object errorMessage)
    {
        if (!expression)
        {
            throw new ArgumentException(string.valueOf(errorMessage));
        }
    }

    /**
     * Ensures the truth of an expression involving one or more parameters to the calling method.
     *
     * @param expression           a bool expression
     * @param errorMessageTemplate a template for the exception message should the check fail. The
     *                             message is formed by replacing each {@code %s} placeholder in the template with an
     *                             argument. These are matched by position - the first {@code %s} gets {@code
     *                             errorMessageArgs[0]}, etc.  Unmatched arguments will be appended to the formatted message
     *                             in square braces. Unmatched placeholders will be left as-is.
     * @param errorMessageArgs     the arguments to be substituted into the message template. Arguments
     *                             are converted to strings using {@link string#valueOf(Object)}.
     * @ if {@code expression} is false
     * @throws NullReferenceException     if the check fails and either {@code errorMessageTemplate} or
     *                                  {@code errorMessageArgs} is null (don't let this happen)
     */
    public static void checkArgument(bool expression,
                                     string errorMessageTemplate,
                                     params object[]  errorMessageArgs)
    {
        if (!expression)
        {
            throw new ArgumentException(Format(errorMessageTemplate, errorMessageArgs));
        }
    }

    /**
     * Ensures the truth of an expression involving the state of the calling instance, but not
     * involving any parameters to the calling method.
     *
     * @param expression a bool expression
     * @throws InvalidOperationException if {@code expression} is false
     */
    public static void checkState(bool expression)
    {
        if (!expression)
        {
            throw new InvalidOperationException();
        }
    }

    /**
     * Ensures the truth of an expression involving the state of the calling instance, but not
     * involving any parameters to the calling method.
     *
     * @param expression   a bool expression
     * @param errorMessage the exception message to use if the check fails; will be converted to a
     *                     string using {@link string#valueOf(Object)}
     * @throws InvalidOperationException if {@code expression} is false
     */
    public static void checkState(bool expression, Object errorMessage)
    {
        if (!expression)
        {
            throw new InvalidOperationException(errorMessage.ToString());
        }
    }

    /**
     * Ensures the truth of an expression involving the state of the calling instance, but not
     * involving any parameters to the calling method.
     *
     * @param expression           a bool expression
     * @param errorMessageTemplate a template for the exception message should the check fail. The
     *                             message is formed by replacing each {@code %s} placeholder in the template with an
     *                             argument. These are matched by position - the first {@code %s} gets {@code
     *                             errorMessageArgs[0]}, etc.  Unmatched arguments will be appended to the formatted message
     *                             in square braces. Unmatched placeholders will be left as-is.
     * @param errorMessageArgs     the arguments to be substituted into the message template. Arguments
     *                             are converted to strings using {@link string#valueOf(Object)}.
     * @throws InvalidOperationException if {@code expression} is false
     * @throws NullReferenceException  if the check fails and either {@code errorMessageTemplate} or
     *                               {@code errorMessageArgs} is null (don't let this happen)
     */
    public static void checkState(bool expression,
                                  string errorMessageTemplate,
                                  params object[]  errorMessageArgs)
    {
        if (!expression)
        {
            throw new InvalidOperationException(Format(errorMessageTemplate, errorMessageArgs));
        }
    }

    /**
     * Ensures that an object reference passed as a parameter to the calling method is not null.
     *
     * @param reference an object reference
     * @return the non-null reference that was validated
     * @throws NullReferenceException if {@code reference} is null
     */
    public static  T checkNotNull<T>(T reference)
    {
        if (reference == null)
        {
            throw new NullReferenceException();
        }
        return reference;
    }

    /**
     * Ensures that an object reference passed as a parameter to the calling method is not null.
     *
     * @param reference    an object reference
     * @param errorMessage the exception message to use if the check fails; will be converted to a
     *                     string using {@link string#valueOf(Object)}
     * @return the non-null reference that was validated
     * @throws NullReferenceException if {@code reference} is null
     */
    public static T checkNotNull<T>(T reference, Object errorMessage)
    {
        if (reference == null)
        {
            throw new NullReferenceException(string.valueOf(errorMessage));
        }
        return reference;
    }

    /**
     * Ensures that an object reference passed as a parameter to the calling method is not null.
     *
     * @param reference            an object reference
     * @param errorMessageTemplate a template for the exception message should the check fail. The
     *                             message is formed by replacing each {@code %s} placeholder in the template with an
     *                             argument. These are matched by position - the first {@code %s} gets {@code
     *                             errorMessageArgs[0]}, etc.  Unmatched arguments will be appended to the formatted message
     *                             in square braces. Unmatched placeholders will be left as-is.
     * @param errorMessageArgs     the arguments to be substituted into the message template. Arguments
     *                             are converted to strings using {@link string#valueOf(Object)}.
     * @return the non-null reference that was validated
     * @throws NullReferenceException if {@code reference} is null
     */
    public static  T checkNotNull<T>(T reference,
                                     string errorMessageTemplate,
                                     params Object[] errorMessageArgs)
    {
        if (reference == null)
        {
            // If either of these parameters is null, the right thing happens anyway
            throw new NullReferenceException(Format(errorMessageTemplate, errorMessageArgs));
        }
        return reference;
    }

  /*
   * All recent hotspots (as of 2009) *really* like to have the natural code
   *
   * if (guardExpression) {
   *    throw new BadException(messageExpression);
   * }
   *
   * refactored so that messageExpression is moved to a separate string-returning method.
   *
   * if (guardExpression) {
   *    throw new BadException(badMsg(...));
   * }
   *
   * The alternative natural refactorings into void or Exception-returning methods are much slower.
   * This is a big deal - we're talking factors of 2-8 in microbenchmarks, not just 10-20%.  (This
   * is a hotspot optimizer bug, which should be fixed, but that's a separate, big project).
   *
   * The coding pattern above is heavily used in java.util, e.g. in ArrayList.  There is a
   * RangeCheckMicroBenchmark in the JDK that was used to test this.
   *
   * But the methods in this class want to throw different exceptions, depending on the args, so it
   * appears that this pattern is not directly applicable.  But we can use the ridiculous, devious
   * trick of throwing an exception in the middle of the construction of another exception.  Hotspot
   * is fine with that.
   */

    /**
     * Ensures that {@code index} specifies a valid <i>element</i> in an array, list or string of size
     * {@code size}. An element index may range from zero, inclusive, to {@code size}, exclusive.
     *
     * @param index a user-supplied index identifying an element of an array, list or string
     * @param size  the size of that array, list or string
     * @return the value of {@code index}
     * @throws IndexOutOfRangeException if {@code index} is negative or is not less than {@code size}
     * @  if {@code size} is negative
     */
    public static int checkElementIndex(int index, int size)
    {
        return checkElementIndex(index, size, "index");
    }

    /**
     * Ensures that {@code index} specifies a valid <i>element</i> in an array, list or string of size
     * {@code size}. An element index may range from zero, inclusive, to {@code size}, exclusive.
     *
     * @param index a user-supplied index identifying an element of an array, list or string
     * @param size  the size of that array, list or string
     * @param desc  the text to use to describe this index in an error message
     * @return the value of {@code index}
     * @throws IndexOutOfRangeException if {@code index} is negative or is not less than {@code size}
     * @  if {@code size} is negative
     */
    public static int checkElementIndex(
            int index, int size, string desc)
    {
        // Carefully optimized for execution by hotspot (explanatory comment above)
        if (index < 0 || index >= size)
        {
            throw new IndexOutOfRangeException(badElementIndex(index, size, desc));
        }
        return index;
    }

    private static string badElementIndex(int index, int size, string desc)
    {
        if (index < 0)
        {
            return Format("%s (%s) must not be negative", desc, index);
        }
        else if (size < 0)
        {
            throw new ArgumentException("negative size: " + size);
        }
        else
        { // index >= size
            return Format("%s (%s) must be less than size (%s)", desc, index, size);
        }
    }

    /**
     * Ensures that {@code index} specifies a valid <i>position</i> in an array, list or string of
     * size {@code size}. A position index may range from zero to {@code size}, inclusive.
     *
     * @param index a user-supplied index identifying a position in an array, list or string
     * @param size  the size of that array, list or string
     * @return the value of {@code index}
     * @throws IndexOutOfRangeException if {@code index} is negative or is greater than {@code size}
     * @  if {@code size} is negative
     */
    public static int checkPositionIndex(int index, int size)
    {
        return checkPositionIndex(index, size, "index");
    }

    /**
     * Ensures that {@code index} specifies a valid <i>position</i> in an array, list or string of
     * size {@code size}. A position index may range from zero to {@code size}, inclusive.
     *
     * @param index a user-supplied index identifying a position in an array, list or string
     * @param size  the size of that array, list or string
     * @param desc  the text to use to describe this index in an error message
     * @return the value of {@code index}
     * @throws IndexOutOfRangeException if {@code index} is negative or is greater than {@code size}
     * @  if {@code size} is negative
     */
    public static int checkPositionIndex(int index, int size, string desc)
    {
        // Carefully optimized for execution by hotspot (explanatory comment above)
        if (index < 0 || index > size)
        {
            throw new IndexOutOfRangeException(badPositionIndex(index, size, desc));
        }
        return index;
    }

    private static string badPositionIndex(int index, int size, string desc)
    {
        if (index < 0)
        {
            return Format("%s (%s) must not be negative", desc, index);
        }
        else if (size < 0)
        {
            throw new ArgumentException("negative size: " + size);
        }
        else
        { // index > size
            return Format("%s (%s) must not be greater than size (%s)", desc, index, size);
        }
    }

    /**
     * Ensures that {@code start} and {@code end} specify a valid <i>positions</i> in an array, list
     * or string of size {@code size}, and are in order. A position index may range from zero to
     * {@code size}, inclusive.
     *
     * @param start a user-supplied index identifying a starting position in an array, list or string
     * @param end   a user-supplied index identifying a ending position in an array, list or string
     * @param size  the size of that array, list or string
     * @throws IndexOutOfRangeException if either index is negative or is greater than {@code size},
     *                                   or if {@code end} is less than {@code start}
     * @  if {@code size} is negative
     */
    public static void checkPositionIndexes(int start, int end, int size)
    {
        // Carefully optimized for execution by hotspot (explanatory comment above)
        if (start < 0 || end < start || end > size)
        {
            throw new IndexOutOfRangeException(badPositionIndexes(start, end, size));
        }
    }

    private static string badPositionIndexes(int start, int end, int size)
    {
        if (start < 0 || start > size)
        {
            return badPositionIndex(start, size, "start index");
        }
        if (end < 0 || end > size)
        {
            return badPositionIndex(end, size, "end index");
        }
        // end < start
        return Format("end index (%s) must not be less than start index (%s)", end, start);
    }

    /**
     * Substitutes each {@code %s} in {@code template} with an argument. These are matched by
     * position: the first {@code %s} gets {@code args[0]}, etc.  If there are more arguments than
     * placeholders, the unmatched arguments will be appended to the end of the formatted message in
     * square braces.
     *
     * @param template a non-null string containing 0 or more {@code %s} placeholders.
     * @param args     the arguments to be substituted into the message template. Arguments are converted
     *                 to strings using {@link string#valueOf(Object)}. Arguments can be null.
     */
    // Note that this is somewhat-improperly used from Verify.java as well.
    static string Format(string template, params object[]  args)
    {
        template = template??"null"; // null -> "null"

        // start substituting the arguments into the '%s' placeholders
        StringBuilder builder = new StringBuilder(template.Length + 16 * args.Length);
        int templateStart = 0;
        int i = 0;
        while (i < args.Length)
        {
            int placeholderStart = template.IndexOf("%s", templateStart);
            if (placeholderStart == -1)
            {
                break;
            }
            builder.Append(template.substring(templateStart, placeholderStart));
            builder.Append(args[i++]);
            templateStart = placeholderStart + 2;
        }
        builder.Append(template.substring(templateStart));

        // if we run Out of placeholders, Append the extra args in square braces
        if (i < args.Length)
        {
            builder.Append(" [");
            builder.Append(args[i++]);
            while (i < args.Length)
            {
                builder.Append(", ");
                builder.Append(args[i++]);
            }
            builder.Append(']');
        }

        return builder.ToString();
    }
}