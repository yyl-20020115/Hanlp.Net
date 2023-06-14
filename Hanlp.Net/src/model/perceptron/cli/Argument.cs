/*
 * Copyright (c) 2005, Sam Pullara. All Rights Reserved.
 * You may modify and redistribute as long as this attribution remains.
 */

namespace com.hankcs.hanlp.model.perceptron.cli;


@Documented
@Retention(RetentionPolicy.RUNTIME)
public @interface Argument
{
    /**
     * This is the actual command line argument itself
     */
    String value() default "";

    /**
     * If this is true, then the argument must be set or the parse will fail
     */
    bool required() default false;

    /**
     * This is the prefix expected for the argument
     */
    String prefix() default "-";

    /**
     * Each argument can have an alias
     */
    String alias() default "";

    /**
     * A description of the argument that will appear in the usage method
     */
    String description() default "";

    /**
     * A delimiter for arguments that are multi-valued.
     */
    String delimiter() default ",";
}