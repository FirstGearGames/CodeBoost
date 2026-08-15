using System;
using System.Collections.Generic;
using Xunit;

namespace CodeBoost.Tests;

/// <summary>
/// Coverage for the type walk behind <c>Polyfill.IsReferenceOrContainsReferences</c>, which stands in for
/// <c>RuntimeHelpers.IsReferenceOrContainsReferences</c> on targets whose runtime does not expose it.
/// </summary>
/// <remarks>
/// <para>
/// These call the walk directly rather than through <c>Polyfill.IsReferenceOrContainsReferences</c>, because this assembly runs on a
/// target that dispatches to the intrinsic instead. Going through the public entry point would test the runtime, not the fallback, and
/// the fallback would ship having never run anywhere.
/// </para>
/// <para>
/// It had no guard against a type reaching itself. A primitive declares a field of its own type, so <c>typeof(int)</c> walks into
/// <c>m_value</c>, which is an <c>int</c>, forever; the pool that asked whether it could skip clearing its array took the stack down
/// with it. Enums do the same through their backing field.
/// </para>
/// </remarks>
public class PolyfillReferenceCheckTests
{
    /// <summary>
    /// Every primitive answers false without recursing into itself, which is the case that used to overflow the stack.
    /// </summary>
    [Fact]
    public void Primitives_AnswerFalse_WithoutRecursingForever()
    {
        Assert.False(Polyfill.ReferenceCheck<int>.Result);
        Assert.False(Polyfill.ReferenceCheck<byte>.Result);
        Assert.False(Polyfill.ReferenceCheck<long>.Result);
        Assert.False(Polyfill.ReferenceCheck<float>.Result);
        Assert.False(Polyfill.ReferenceCheck<double>.Result);
        Assert.False(Polyfill.ReferenceCheck<bool>.Result);
        Assert.False(Polyfill.ReferenceCheck<char>.Result);
    }

    /// <summary>
    /// An enum answers false, which it reaches through its backing field and so would have recursed the same way.
    /// </summary>
    [Fact]
    public void Enum_AnswersFalse()
    {
        Assert.False(Polyfill.ReferenceCheck<ProbeEnum>.Result);
    }

    /// <summary>
    /// A struct of primitives answers false, so the walk descends rather than answering from the outer type alone.
    /// </summary>
    [Fact]
    public void StructOfPrimitives_AnswersFalse()
    {
        Assert.False(Polyfill.ReferenceCheck<PlainStruct>.Result);
        Assert.False(Polyfill.ReferenceCheck<Guid>.Result);
    }

    /// <summary>
    /// A reference type answers true immediately.
    /// </summary>
    [Fact]
    public void ReferenceTypes_AnswerTrue()
    {
        Assert.True(Polyfill.ReferenceCheck<string>.Result);
        Assert.True(Polyfill.ReferenceCheck<object>.Result);
        Assert.True(Polyfill.ReferenceCheck<List<int>>.Result);
    }

    /// <summary>
    /// A struct holding a reference answers true, and so does one holding it only through another struct, which is the whole reason
    /// the walk is recursive.
    /// </summary>
    [Fact]
    public void StructHoldingAReference_AnswersTrue()
    {
        Assert.True(Polyfill.ReferenceCheck<StructWithReference>.Result);
        Assert.True(Polyfill.ReferenceCheck<StructNestingAReference>.Result);
    }

    /// <summary>
    /// The answer matches the runtime intrinsic this stands in for, on every shape above.
    /// </summary>
    /// <remarks>The point of a polyfill is to be indistinguishable from the thing it replaces, and this assembly can run both.</remarks>
    [Fact]
    public void Walk_AgreesWithTheRuntimeIntrinsic()
    {
        Assert.Equal(System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<int>(), Polyfill.ReferenceCheck<int>.Result);
        Assert.Equal(System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<ProbeEnum>(), Polyfill.ReferenceCheck<ProbeEnum>.Result);
        Assert.Equal(System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<PlainStruct>(), Polyfill.ReferenceCheck<PlainStruct>.Result);
        Assert.Equal(System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<Guid>(), Polyfill.ReferenceCheck<Guid>.Result);
        Assert.Equal(System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<string>(), Polyfill.ReferenceCheck<string>.Result);
        Assert.Equal(System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<StructWithReference>(), Polyfill.ReferenceCheck<StructWithReference>.Result);
        Assert.Equal(System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<StructNestingAReference>(), Polyfill.ReferenceCheck<StructNestingAReference>.Result);
    }

    /// <summary>
    /// The premise the primitive guard rests on: a primitive really does declare a field of its own type, so a walk without that
    /// guard really would recurse forever rather than merely being untidy.
    /// </summary>
    /// <remarks>Asserted rather than assumed, because the guard above looks like defensive noise until this is in front of you.</remarks>
    [Fact]
    public void Primitive_DeclaresAFieldOfItsOwnType()
    {
        System.Reflection.FieldInfo[] fields = typeof(int).GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

        Assert.NotEmpty(fields);
        Assert.Contains(fields, field => field.FieldType == typeof(int));
    }

    /// <summary>
    /// An enum whose backing field is the recursion the primitive guard stops.
    /// </summary>
    private enum ProbeEnum : int
    {
        /// <summary>The only value; the walk cares about the layout rather than the members.</summary>
        None = 0,
    }

    /// <summary>
    /// A value type holding nothing but primitives.
    /// </summary>
    private struct PlainStruct
    {
        /// <summary>A primitive field, which the walk must descend into and answer false for.</summary>
        public int Value;
        /// <summary>A second primitive of another width, so the walk cannot pass by inspecting one field.</summary>
        public double Scale;
    }

    /// <summary>
    /// A value type holding a reference directly.
    /// </summary>
    private struct StructWithReference
    {
        /// <summary>The reference that makes the answer true.</summary>
        public string Name;
    }

    /// <summary>
    /// A value type whose only reference is reached through another value type, so a walk that stopped at the first level would miss it.
    /// </summary>
    private struct StructNestingAReference
    {
        /// <summary>A primitive, so the reference is not the first field found.</summary>
        public int Value;
        /// <summary>The nested struct carrying the reference.</summary>
        public StructWithReference Nested;
    }
}
