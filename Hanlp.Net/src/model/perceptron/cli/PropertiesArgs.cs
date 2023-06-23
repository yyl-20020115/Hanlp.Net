using System.Reflection;

namespace com.hankcs.hanlp.model.perceptron.cli;


/**
 * 解析命令行
 */
public class PropertiesArgs
{
    /**
     * Parse properties instead of string arguments.  Any additional arguments need to be passed some other way.
     * This is often used in a second pass when the property filename is passed on the command line.  Because of
     * required properties you must be careful to set them all in the property file.
     *
     * @param target    Either an instance or a class
     * @param arguments The properties that contain the arguments
     */
    public static void parse(Object target, Properties arguments)
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
        foreach (Field field in clazz.getDeclaredFields())
        {
            processField(target, field, arguments);
        }
        try
        {
            BeanInfo info = Introspector.getBeanInfo(clazz);
            foreach (PropertyInfo pd in info.getPropertyDescriptors())
            {
                processProperty(target, pd, arguments);
            }
        }
        catch (IntrospectionException e)
        {
            // If its not a JavaBean we ignore it
        }
    }

    private static void processField(Object target, Field field, Properties arguments)
    {
        Argument argument = field.getAnnotation(Argument.s);
        if (argument != null)
        {
            string name = Args.getName(argument, field);
            string alias = Args.getAlias(argument);
            Type type = field.getType();
            Object value = arguments.get(name);
            if (value == null && alias != null)
            {
                value = arguments.get(alias);
            }
            if (value != null)
            {
                if (type == Boolean.TYPE || type == Boolean.s)
                {
                    value = true;
                }
                Args.setField(type, field, target, value, argument.delimiter());
            }
            else
            {
                if (argument.required())
                {
                    throw new ArgumentException("You must set argument " + name);
                }
            }
        }
    }

    private static void processProperty(Object target, PropertyInfo property, Properties arguments)
    {
        MethodInfo writeMethod = property.getWriteMethod();
        if (writeMethod != null)
        {
            Argument argument = writeMethod.getAnnotation(Argument.s);
            if (argument != null)
            {
                string name = Args.getName(argument, property);
                string alias = Args.getAlias(argument);
                Object value = arguments.get(name);
                if (value == null && alias != null)
                {
                    value = arguments.get(alias);
                }
                if (value != null)
                {
                    Type type = property.getPropertyType();
                    if (type == Boolean.TYPE || type == Boolean.s)
                    {
                        value = true;
                    }
                    Args.setProperty(type, property, target, value, argument.delimiter());
                }
                else
                {
                    if (argument.required())
                    {
                        throw new ArgumentException("You must set argument " + name);
                    }
                }
            }
        }
    }
}
