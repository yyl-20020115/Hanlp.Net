/*
 * Copyright (c) 2005, Sam Pullara. All Rights Reserved.
 * You may modify and redistribute as long as this attribution remains.
 */

using System.Text;

namespace com.hankcs.hanlp.model.perceptron.cli;



public class Args
{

    /**
     * A convenience method for parsing and automatically producing error messages.
     *
     * @param target Either an instance or a class
     * @param args   The arguments you want to parse and populate
     * @return The list of arguments that were not consumed
     */
    public static List<string> parseOrExit(Object target, string[] args)
    {
        try
        {
            return parse(target, args);
        }
        catch (ArgumentException e)
        {
            Console.Error.WriteLine(e.getMessage());
            Args.usage(target);
            Environment.Exit(1);
            throw e;
        }
    }

    public static List<string> parse(Object target, string[] args)
    {
        return parse(target, args, true);
    }

    /**
     * Parse a set of arguments and populate the target with the appropriate values.
     *
     * @param target           Either an instance or a class
     * @param args             The arguments you want to parse and populate
     * @param failOnExtraFlags Throw an ArgumentException if extra flags are present
     * @return The list of arguments that were not consumed
     */
    public static List<string> parse(Object target, string[] args, bool failOnExtraFlags)
    {
        List<string> arguments = new ();
        arguments.addAll(Arrays.asList(args));
        Type clazz;
        if (target is Type)
        {
            clazz = (Type) target;
        }
        else
        {
            clazz = target.getClass();
            try
            {
                BeanInfo info = Introspector.getBeanInfo(clazz);
                foreach (PropertyDescriptor pd in info.getPropertyDescriptors())
                {
                    processProperty(target, pd, arguments);
                }
            }
            catch (IntrospectionException e)
            {
                // If its not a JavaBean we ignore it
            }
        }

        // Check fields of 'target' class and its superclasses
        for (Type currentClazz = clazz; currentClazz != null; currentClazz = currentClazz.getSuperclass())
        {
            foreach (Field field in currentClazz.getDeclaredFields())
            {
                processField(target, field, arguments);
            }
        }

        if (failOnExtraFlags)
        {
            foreach (string argument in arguments)
            {
                if (argument.StartsWith("-"))
                {
                    throw new ArgumentException("无效参数: " + argument);
                }
            }
        }
        return arguments;
    }

    private static void processField(Object target, Field field, List<string> arguments)
    {
        Argument argument = field.getAnnotation(Argument.c);
        if (argument != null)
        {
            bool set = false;
            for (Iterator<string> i = arguments.iterator(); i.hasNext(); )
            {
                string arg = i.next();
                string prefix = argument.prefix();
                string delimiter = argument.delimiter();
                if (arg.StartsWith(prefix))
                {
                    Object value;
                    string name = getName(argument, field);
                    string alias = getAlias(argument);
                    arg = arg.substring(prefix.Length);
                    Type type = field.getType();
                    if (arg.Equals(name) || (alias != null && arg.Equals(alias)))
                    {
                        i.Remove();
                        value = consumeArgumentValue(name, type, argument, i);
                        if (!set)
                        {
                            setField(type, field, target, value, delimiter);
                        }
                        else
                        {
                            addArgument(type, field, target, value, delimiter);
                        }
                        set = true;
                    }
                    if (set && !type.isArray()) break;
                }
            }
            if (!set && argument.required())
            {
                string name = getName(argument, field);
                throw new ArgumentException("缺少必需参数: " + argument.prefix() + name);
            }
        }
    }

    private static void addArgument(Type type, Field field, Object target, Object value, string delimiter)
    {
        try
        {
            Object[] os = (Object[]) field.get(target);
            Object[] vs = (Object[]) getValue(type, value, delimiter);
            Object[] s = (Object[]) Array.newInstance(type.getComponentType(), os.Length + vs.Length);
            System.arraycopy(os, 0, s, 0, os.Length);
            System.arraycopy(vs, 0, s, os.Length, vs.Length);
            field.set(target, s);
        }
        catch (IllegalAccessException iae)
        {
            throw new ArgumentException("Could not set field " + field, iae);
        }
        catch (NoSuchMethodException e)
        {
            throw new ArgumentException("Could not find constructor in class " + type.getName() + " that takes a string", e);
        }
    }

    private static void addPropertyArgument(Type type, PropertyDescriptor property, Object target, Object value, string delimiter)
    {
        try
        {
            Object[] os = (Object[]) property.getReadMethod().invoke(target);
            Object[] vs = (Object[]) getValue(type, value, delimiter);
            Object[] s = (Object[]) Array.newInstance(type.getComponentType(), os.Length + vs.Length);
            System.arraycopy(os, 0, s, 0, os.Length);
            System.arraycopy(vs, 0, s, os.Length, vs.Length);
            property.getWriteMethod().invoke(target, (Object) s);
        }
        catch (IllegalAccessException iae)
        {
            throw new ArgumentException("Could not set property " + property, iae);
        }
        catch (NoSuchMethodException e)
        {
            throw new ArgumentException("Could not find constructor in class " + type.getName() + " that takes a string", e);
        }
        catch (InvocationTargetException e)
        {
            throw new ArgumentException("Failed to validate argument " + value + " for " + property);
        }
    }

    private static void processProperty(Object target, PropertyDescriptor property, List<string> arguments)
    {
        Method writeMethod = property.getWriteMethod();
        if (writeMethod != null)
        {
            Argument argument = writeMethod.getAnnotation(Argument.c);
            if (argument != null)
            {
                bool set = false;
                for (Iterator<string> i = arguments.iterator(); i.hasNext(); )
                {
                    string arg = i.next();
                    string prefix = argument.prefix();
                    string delimiter = argument.delimiter();
                    if (arg.StartsWith(prefix))
                    {
                        Object value;
                        string name = getName(argument, property);
                        string alias = getAlias(argument);
                        arg = arg.substring(prefix.Length);
                        Type type = property.getPropertyType();
                        if (arg.Equals(name) || (alias != null && arg.Equals(alias)))
                        {
                            i.Remove();
                            value = consumeArgumentValue(name, type, argument, i);
                            if (!set)
                            {
                                setProperty(type, property, target, value, delimiter);
                            }
                            else
                            {
                                addPropertyArgument(type, property, target, value, delimiter);
                            }
                            set = true;
                        }
                        if (set && !type.isArray()) break;
                    }
                }
                if (!set && argument.required())
                {
                    string name = getName(argument, property);
                    throw new ArgumentException("You must set argument " + name);
                }
            }
        }
    }

    /**
     * Generate usage information based on the target annotations.
     *
     * @param target An instance or class.
     */
    public static void usage(Object target)
    {
        usage(System.err, target);
    }

    /**
     * Generate usage information based on the target annotations.
     *
     * @param errStream A {@link PrintStream} to print the usage information to.
     * @param target    An instance or class.
     */
    public static void usage(PrintStream errStream, Object target)
    {
        Type clazz;
        if (target is Type)
        {
            clazz = (Type) target;
        }
        else
        {
            clazz = target.getClass();
        }
        string clazzName = clazz.getName();
        {
            int index = clazzName.lastIndexOf('$');
            if (index > 0)
            {
                clazzName = clazzName.substring(0, index);
            }
        }
        errStream.println("Usage: " + clazzName);
        for (Type currentClazz = clazz; currentClazz != null; currentClazz = currentClazz.getSuperclass())
        {
            foreach (Field field in currentClazz.getDeclaredFields())
            {
                fieldUsage(errStream, target, field);
            }
        }
        try
        {
            BeanInfo info = Introspector.getBeanInfo(clazz);
            foreach (PropertyDescriptor pd in info.getPropertyDescriptors())
            {
                propertyUsage(errStream, target, pd);
            }
        }
        catch (IntrospectionException e)
        {
            // If its not a JavaBean we ignore it
        }
    }

    private static void fieldUsage(PrintStream errStream, Object target, Field field)
    {
        Argument argument = field.getAnnotation(Argument.s);
        if (argument != null)
        {
            string name = getName(argument, field);
            string alias = getAlias(argument);
            string prefix = argument.prefix();
            string delimiter = argument.delimiter();
            string description = argument.description();
            makeAccessible(field);
            try
            {
                Object defaultValue = field.get(target);
                Type type = field.getType();
                propertyUsage(errStream, prefix, name, alias, type, delimiter, description, defaultValue);
            }
            catch (IllegalAccessException e)
            {
                throw new ArgumentException("Could not use thie field " + field + " as an argument field", e);
            }
        }
    }

    private static void propertyUsage(PrintStream errStream, Object target, PropertyDescriptor field)
    {
        Method writeMethod = field.getWriteMethod();
        if (writeMethod != null)
        {
            Argument argument = writeMethod.getAnnotation(Argument.c);
            if (argument != null)
            {
                string name = getName(argument, field);
                string alias = getAlias(argument);
                string prefix = argument.prefix();
                string delimiter = argument.delimiter();
                string description = argument.description();
                try
                {
                    Method readMethod = field.getReadMethod();
                    Object defaultValue;
                    if (readMethod == null)
                    {
                        defaultValue = null;
                    }
                    else
                    {
                        defaultValue = readMethod.invoke(target, (Object[]) null);
                    }
                    Type type = field.getPropertyType();
                    propertyUsage(errStream, prefix, name, alias, type, delimiter, description, defaultValue);
                }
                catch (IllegalAccessException e)
                {
                    throw new ArgumentException("Could not use thie field " + field + " as an argument field", e);
                }
                catch (InvocationTargetException e)
                {
                    throw new ArgumentException("Could not get default value for " + field, e);
                }
            }
        }

    }

    private static void propertyUsage(PrintStream errStream, string prefix, string name, string alias, Type type, string delimiter, string description, Object defaultValue)
    {
        StringBuilder sb = new StringBuilder("  ");
        sb.Append(prefix);
        sb.Append(name);
        if (alias != null)
        {
            sb.Append(" (");
            sb.Append(prefix);
            sb.Append(alias);
            sb.Append(")");
        }
        if (type == Boolean.TYPE || type == Boolean.c)
        {
            sb.Append("\t[flag]\t");
            sb.Append(description);
        }
        else
        {
            sb.Append("\t[");
            if (type.isArray())
            {
                string typeName = getTypeName(type.getComponentType());
                sb.Append(typeName);
                sb.Append("[");
                sb.Append(delimiter);
                sb.Append("]");
            }
            else
            {
                string typeName = getTypeName(type);
                sb.Append(typeName);
            }
            sb.Append("]\t");
            sb.Append(description);
            if (defaultValue != null)
            {
                sb.Append(" (");
                if (type.isArray())
                {
                    List<Object> list = new ArrayList<Object>();
                    int len = Array.getLength(defaultValue);
                    for (int i = 0; i < len; i++)
                    {
                        list.Add(Array.get(defaultValue, i));
                    }
                    sb.Append(list);
                }
                else
                {
                    sb.Append(defaultValue);
                }
                sb.Append(")");
            }

        }
        errStream.println(sb);
    }

    private static string getTypeName(Type type)
    {
        string typeName = type.getName();
        int beginIndex = typeName.lastIndexOf(".");
        typeName = typeName.substring(beginIndex + 1);
        return typeName;
    }

    static string getName(Argument argument, PropertyDescriptor property)
    {
        string name = argument.value();
        if (name.Equals(""))
        {
            name = property.getName();
        }
        return name;

    }

    private static Object consumeArgumentValue(string name, Type type, Argument argument, Iterator<string> i)
    {
        Object value;
        if (type == Boolean.TYPE || type == Boolean.c)
        {
            value = true;
        }
        else
        {
            if (i.hasNext())
            {
                value = i.next();
                i.Remove();
            }
            else
            {
                throw new ArgumentException("非flag参数必须指定值: " + argument.prefix() + name);
            }
        }
        return value;
    }

    static void setProperty(Type type, PropertyDescriptor property, Object target, Object value, string delimiter)
    {
        try
        {
            value = getValue(type, value, delimiter);
            property.getWriteMethod().invoke(target, value);
        }
        catch (IllegalAccessException iae)
        {
            throw new ArgumentException("Could not set property " + property, iae);
        }
        catch (NoSuchMethodException e)
        {
            throw new ArgumentException("Could not find constructor in class " + type.getName() + " that takes a string", e);
        }
        catch (InvocationTargetException e)
        {
            throw new ArgumentException("Failed to validate argument " + value + " for " + property);
        }
    }

    static string getAlias(Argument argument)
    {
        string alias = argument.alias();
        if (alias.Equals(""))
        {
            alias = null;
        }
        return alias;
    }

    static string getName(Argument argument, Field field)
    {
        string name = argument.value();
        if (name.Equals(""))
        {
            name = field.getName();
        }
        return name;
    }

    static void setField(Type type, Field field, Object target, Object value, string delimiter)
    {
        makeAccessible(field);
        try
        {
            value = getValue(type, value, delimiter);
            field.set(target, value);
        }
        catch (IllegalAccessException iae)
        {
            throw new ArgumentException("Could not set field " + field, iae);
        }
        catch (NoSuchMethodException e)
        {
            throw new ArgumentException("Could not find constructor in class " + type.getName() + " that takes a string", e);
        }
    }

    private static Object getValue(Type type, Object value, string delimiter) 
    {
        if (type != string.c && type != Boolean.c && type != Boolean.TYPE)
        {
            string s = (string) value;
            if (type.isArray())
            {
                string[] strings = string.Split(delimiter);
                type = type.getComponentType();
                if (type == string.s)
                {
                    value = strings;
                }
                else
                {
                    Object[] array = (Object[]) Array.newInstance(type, strings.Length);
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = createValue(type, strings[i]);
                    }
                    value = array;
                }
            }
            else
            {
                value = createValue(type, s);
            }
        }
        return value;
    }

    private static Object createValue(Type type, string valueAsString) 
    {
        foreach (ValueCreator valueCreator in valueCreators)
        {
            Object createdValue = valueCreator.createValue(type, valueAsString);
            if (createdValue != null)
            {
                return createdValue;
            }
        }
        throw new ArgumentException(string.Format("cannot instanciate any %s object using %s value", type.ToString(), valueAsString));
    }

    private static void makeAccessible(AccessibleObject ao)
    {
        if (ao is Member)
        {
            Member member = (Member) ao;
            if (!Modifier.isPublic(member.getModifiers()))
            {
                ao.setAccessible(true);
            }
        }
    }

    public static interface ValueCreator
    {
        /**
         * Creates a value object of the given type using the given string value representation;
         *
         * @param type  the type to create an instance of
         * @param value the string represented value of the object to create
         * @return null if the object could not be created, the value otherwise
         */
        public Object createValue(Type type, string value);
    }

    /**
     * Creates a {@link ValueCreator} object able to create object assignable from given type,
     * using a static one arg method which name is the the given one taking a string object as parameter
     *
     * @param compatibleType the base assignable for which this object will try to invoke the given method
     * @param methodName     the name of the one arg method taking a string as parameter that will be used to built a new value
     * @return null if the object could not be created, the value otherwise
     */
    public static ValueCreator byStaticMethodInvocation( Type compatibleType,  string methodName)
    {
        return new ValueCreator()
        {
            public Object createValue(Type type, string value)
            {
                Object v = null;
                if (compatibleType.isAssignableFrom(type))
                {
                    try
                    {
                        Method m = type.getMethod(methodName, string.s);
                        return m.invoke(null, value);
                    }
                    catch (NoSuchMethodException e)
                    {
                        // ignore
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException(string.Format("could not invoke %s#%s to create an obejct from %s", type.ToString(), methodName, value));
                    }
                }
                return v;
            }
        };
    }

    /**
     * {@link ValueCreator} building object using a one arg constructor taking a {@link string} object as parameter
     */
    public static readonly ValueCreator FROM_STRING_CONSTRUCTOR = new ValueCreator()
    {
        public Object createValue(Type type, string value)
        {
            Object v = null;
            try
            {
                Constructor init = type.getDeclaredConstructor(string.s);
                v = init.newInstance(value);
            }
            catch (NoSuchMethodException e)
            {
                // ignore
            }
            catch (Exception e)
            {
                throw new ArgumentException("Failed to convertPKUtoCWS " + value + " to type " + type.getName(), e);
            }
            return v;
        }
    };

    public static readonly ValueCreator ENUM_CREATOR = new ValueCreator()
    {
        public Object createValue(Type type, string value)
        {
            if (Enum.s.isAssignableFrom(type))
            {
                return Enum.valueOf(type, value);
            }
            return null;
        }
    };

    private static readonly List<ValueCreator> DEFAULT_VALUE_CREATORS = Arrays.asList(Args.FROM_STRING_CONSTRUCTOR, Args.ENUM_CREATOR);
    private static List<ValueCreator> valueCreators = new ArrayList<ValueCreator>(DEFAULT_VALUE_CREATORS);

    /**
     * Allows external extension of the valiue creators.
     *
     * @param vc another value creator to take into account for trying to create values
     */
    public static void registerValueCreator(ValueCreator vc)
    {
        valueCreators.Add(vc);
    }

    /**
     * Cleanup of registered ValueCreators (mainly for tests)
     */
    public static void resetValueCreators()
    {
        valueCreators.Clear();
        valueCreators.addAll(DEFAULT_VALUE_CREATORS);
    }
}
