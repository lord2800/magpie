namespace Magpie;

using System;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Class)]
public class AutowireAttribute(Type? substituteType = null) : Attribute
{
    public Type? SubstituteType { get; } = substituteType;
}

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Class)]
public class DalamudWindowAttribute(Type constructorType) : Attribute
{
    public Type ConstructorType { get; } = constructorType;
}

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Method)]
public class InitializerAttribute : Attribute;
[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Method)]
public class ShutdownAttribute : Attribute;
