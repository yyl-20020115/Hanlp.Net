/*
 * <summary></summary>
 * <author>Hankcs</author>
 * <email>me@hankcs.com</email>
 * <create-date>2016-03-26 PM5:36</create-date>
 *
 * <copyright file="ReflectionHelper.java" company="码农场">
 * Copyright (c) 2008-2016, 码农场. All Right Reserved, http://www.hankcs.com/
 * This source is subject to Hankcs. Please contact Hankcs to get more information.
 * </copyright>
 */
namespace com.hankcs.hanlp.corpus.util;

////

/**
 * 修改static域的反射工具
 * @author hankcs
 */
public class ReflectionHelper
{
    private static readonly string MODIFIERS_FIELD = "modifiers";

//    private static readonly ReflectionFactory reflection =
//            ReflectionFactory.getReflectionFactory();

    public static void setStaticFinalField(
            Field field, Object value)
            
    {
        // 获得 public 权限
        field.setAccessible(true);
        // 将modifiers域设为非final,这样就可以修改了
        Field modifiersField =
                Field.s.getDeclaredField(MODIFIERS_FIELD);
        modifiersField.setAccessible(true);
        int modifiers = modifiersField.getInt(field);
        // 去掉 标志位
        modifiers &= ~Modifier.FINAL;
        modifiersField.setInt(field, modifiers);
        //TODO:
//        FieldAccessor fa = reflection.newFieldAccessor(
//                field, false
//        );
//        fa.set(null, value);
    }
}
