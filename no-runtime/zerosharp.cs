using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Internal.Runtime.CompilerServices;

#region A couple very basic things
namespace System
{
    public unsafe class Object
    {
        public Internal.Runtime.EEType* m_pEEType;

        ~Object()
        {
        }

        public virtual string ToString()
        {
            return "object";
        }
    }
    public struct Void { }
    public struct Boolean { }
    public struct Char { }
    public struct SByte { }
    public struct Byte { }
    public struct Int16 { }
    public struct UInt16 { }
    public struct Int32 { }
    public struct UInt32 { }
    public struct Int64 { }
    public struct UInt64 { }
    public struct IntPtr { }
    public struct UIntPtr { }
    public struct Single { }
    public struct Double { }
    public abstract class ValueType { }
    public abstract class Enum : ValueType { }
    public struct Nullable<T> where T : struct { }

    public sealed class String
    {
        private readonly int _stringLength;
        private char _firstChar;

        public String(char[] value)
        {
            while (true) { }
        }
        public int Length
        {
            [Runtime.CompilerServices.Intrinsic]
            get => _stringLength;
        }

        public override string ToString()
        {
            return this;
        }
    }
    public abstract class Array { }
    public abstract class Delegate { }
    public abstract class MulticastDelegate : Delegate { }

    public struct RuntimeTypeHandle { }
    public struct RuntimeMethodHandle { }
    public struct RuntimeFieldHandle { }


    public class Attribute { }

    [AttributeUsage(AttributeTargets.Enum, Inherited = false)]
    public class FlagsAttribute : Attribute
    {
        public FlagsAttribute()
        {
        }
    }

    namespace Runtime.Versioning
    {
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Constructor,
                        AllowMultiple = false, Inherited = false)]
        internal sealed class NonVersionableAttribute : Attribute
        {
            public NonVersionableAttribute()
            {
            }
        }
    }

    namespace Runtime.CompilerServices
    {
        public class RuntimeHelpers
        {
            public static unsafe int OffsetToStringData => sizeof(IntPtr) + sizeof(int);
        }

        [Flags]
        public enum MethodImplOptions
        {
            Unmanaged = 0x0004,
            NoInlining = 0x0008,
            ForwardRef = 0x0010,
            Synchronized = 0x0020,
            NoOptimization = 0x0040,
            PreserveSig = 0x0080,
            AggressiveInlining = 0x0100,
            AggressiveOptimization = 0x0200,
            InternalCall = 0x1000
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field, Inherited = false)]
        internal sealed class IntrinsicAttribute : Attribute
        {
        }

        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
        public sealed class MethodImplAttribute : Attribute
        {
            public MethodImplAttribute(MethodImplOptions methodImplOptions)
            {
                Value = methodImplOptions;
            }

            public MethodImplAttribute(short value)
            {
                Value = (MethodImplOptions)value;
            }

            public MethodImplAttribute()
            {
            }

            public MethodImplOptions Value { get; }
        }
    }


    internal enum AttributeTargets
    {
        Assembly = 0x0001,
        Module = 0x0002,
        Class = 0x0004,
        Struct = 0x0008,
        Enum = 0x0010,
        Constructor = 0x0020,
        Method = 0x0040,
        Property = 0x0080,
        Field = 0x0100,
        Event = 0x0200,
        Interface = 0x0400,
        Parameter = 0x0800,
        Delegate = 0x1000,
        ReturnValue = 0x2000,
        GenericParameter = 0x4000,

        All = Assembly | Module | Class | Struct | Enum | Constructor |
                        Method | Property | Field | Event | Interface | Parameter |
                        Delegate | ReturnValue | GenericParameter
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    internal sealed class AttributeUsageAttribute : Attribute
    {
        //Constructors
        public AttributeUsageAttribute(AttributeTargets validOn)
        {
        }

        public AttributeUsageAttribute(AttributeTargets validOn, bool allowMultiple, bool inherited)
        {
        }

        //Properties.
        // Allowing the set properties as it allows a more readable syntax in the specifiers (and are commonly used)
        // The get properties will be needed only if these attributes are used at Runtime, however, the compiler
        // is getting an internal error if the gets are not defined.

        public bool AllowMultiple
        {
            get { return false; }
            set { }
        }

        public bool Inherited
        {
            get { return false; }
            set { }
        }
    }
}
namespace System.Runtime.InteropServices
{
    public sealed class DllImportAttribute : Attribute
    {
        public DllImportAttribute(string dllName) { }
    }

    public sealed class StructLayoutAttribute : Attribute
    {
        public StructLayoutAttribute(LayoutKind layoutKind)
        {
            Value = layoutKind;
        }

        public StructLayoutAttribute(short layoutKind)
        {
            Value = (LayoutKind)layoutKind;
        }

        public LayoutKind Value { get; }

        public int Pack;
        public int Size;
        public CharSet CharSet;
    }

    public enum LayoutKind
    {
        Sequential = 0,
        Explicit = 2,
        Auto = 3,
    }

    public enum CharSet
    {
        None = 1,        // User didn't specify how to marshal strings.
        Ansi = 2,        // Strings should be marshalled as ANSI 1 byte chars.
        Unicode = 3,     // Strings should be marshalled as Unicode 2 byte chars.
        Auto = 4,        // Marshal Strings in the right way for the target system.
    }

    public sealed class FieldOffsetAttribute : Attribute
    {
        public FieldOffsetAttribute(int offset)
        {
            Value = offset;
        }

        public int Value { get; }
    }
}
#endregion

#region Things needed by ILC
namespace System
{
    namespace Runtime
    {
        internal sealed class RuntimeExportAttribute : Attribute
        {
            public RuntimeExportAttribute(string entry) { }
        }
    }

    class Array<T> : Array { }
}

namespace Internal.Runtime
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct EEType
    {
#if TARGET_64BIT
        private const int POINTER_SIZE = 8;
        private const int PADDING = 1; // _numComponents is padded by one Int32 to make the first element pointer-aligned
#else
        private const int POINTER_SIZE = 4;
        private const int PADDING = 0;
#endif
        internal const int SZARRAY_BASE_SIZE = POINTER_SIZE + POINTER_SIZE + (1 + PADDING) * 4;

        [StructLayout(LayoutKind.Explicit)]
        private unsafe struct RelatedTypeUnion
        {
            // Kinds.CanonicalEEType
            [FieldOffset(0)]
            public EEType* _pBaseType;
            [FieldOffset(0)]
            public EEType** _ppBaseTypeViaIAT;

            // Kinds.ClonedEEType
            [FieldOffset(0)]
            public EEType* _pCanonicalType;
            [FieldOffset(0)]
            public EEType** _ppCanonicalTypeViaIAT;

            // Kinds.ArrayEEType
            [FieldOffset(0)]
            public EEType* _pRelatedParameterType;
            [FieldOffset(0)]
            public EEType** _ppRelatedParameterTypeViaIAT;
        }

        public ushort _usComponentSize;
        public ushort _usFlags;
        public uint _uBaseSize;
        private RelatedTypeUnion _relatedType;
        private ushort _usNumVtableSlots;
        private ushort _usNumInterfaces;
        private uint _uHashCode;
    }
}

namespace Internal.Runtime.CompilerHelpers
{
    class StartupCodeHelpers
    {
        [RuntimeExport("RhpReversePInvoke2")]
        static void RhpReversePInvoke2() { }
        [RuntimeExport("RhpReversePInvokeReturn2")]
        static void RhpReversePInvokeReturn2() { }
        [System.Runtime.RuntimeExport("__fail_fast")]
        static void FailFast() { while (true) ; }
        [System.Runtime.RuntimeExport("RhpPInvoke")]
        static void RphPinvoke() { }
        [System.Runtime.RuntimeExport("RhpPInvokeReturn")]
        static void RphPinvokeReturn() { }


        [DllImport("kernel32")]
        internal static extern IntPtr LocalAlloc(uint uFlags, nuint uBytes);


        [RuntimeExport("RhpNewFast")]
        internal static unsafe object RhpNewFast(Internal.Runtime.EEType* pEEType)
        {
            if (pEEType->_usComponentSize != 0)
            {
                while (true) ;
            }
            IntPtr ptr = LocalAlloc(0 /*fixed*/, pEEType->_uBaseSize);
            object obj = Unsafe.AsRef<object>(&ptr);
            obj.m_pEEType = pEEType;
            return obj;
        }
    }

    class ThrowHelpers
    {
        private static void ThrowTypeLoadException()
        {
            while (true)
            {
            }
        }

        public static void ThrowTypeLoadExceptionWithArgument(Internal.TypeSystem.ExceptionStringID id, string className, string typeName, string messageArg)
        {
            while (true)
            {
            }
        }
    }
}
namespace Internal.Runtime.CompilerServices
{
    public static unsafe class Unsafe
    {
        [Intrinsic]
        [NonVersionable]
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static T As<T>(object value) where T : class
        {
            while (true) ;

            // ldarg.0
            // ret
        }

        [Intrinsic]
        [NonVersionable]
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static ref T AsRef<T>(void* source)
        {
            return ref Unsafe.As<byte, T>(ref *(byte*)source);
        }

        [Intrinsic]
        [NonVersionable]
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static ref TTo As<TFrom, TTo>(ref TFrom source)
        {
            while (true) ;

            // ldarg.0
            // ret
        }
    }

}
namespace Internal.TypeSystem
{
    /// <summary>
    /// Represents an ID of a localized exception string.
    /// </summary>
    public enum ExceptionStringID
    {
    }
}

#endregion

unsafe class Program
{
    [DllImport("kernel32")]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32")]
    static extern IntPtr WriteConsoleW(IntPtr hConsole, void* lpBuffer, int charsToWrite, out int charsWritten, void* reserved);

    [MethodImplAttribute(MethodImplOptions.NoInlining)]
    static object Factory()
    {
        return new object();
    }

    [RuntimeExport("wmainCRTStartup")]
    static int Main()
    {
        string hello = Factory().ToString();
        fixed (char* c = hello)
        {
            int charsWritten;
            WriteConsoleW(GetStdHandle(-11), c, hello.Length, out charsWritten, null);
        }

        return 42;
    }
}
