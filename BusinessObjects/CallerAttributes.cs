
// PLEASE NOTE
//
// This is only needed for .NET Framework 3.5 and 4.0 compatibility.
// Project needs to be compiled with VS2012+.
//
// If you want to target .NET 4.5+ then you might choose to exclude this file
// (if you don't you will only get a few duplicate definition warnings and no compile errors).

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    [AttributeUsageAttribute(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerMemberNameAttribute : Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerFilePathAttribute : Attribute
    {
    }

    [AttributeUsageAttribute(AttributeTargets.Parameter, Inherited = false)]
    public sealed class CallerLineNumberAttribute : Attribute
    {
    }
}