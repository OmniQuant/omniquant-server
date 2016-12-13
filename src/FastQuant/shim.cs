#if NETSTANDARD1_3

using System.Runtime.InteropServices;

namespace System
{
    // FIXME: will be removed when netstandard 2.0 is ready
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
    [ComVisible(true)]
    public sealed class SerializableAttribute : Attribute
    { }

    // FIXME: will be removed when netstandard 2.0 is ready
    [Serializable]
    [ComVisible(true)]
    public class ApplicationException : Exception
    {
        public ApplicationException(string message)
        { }
    }
}

namespace System.Drawing
{   
    // FIXME: will be removed when netstandard 2.0 is ready
    public struct Color
    {
        public static Color Red => FromArgb(0);

        public static Color Green => FromArgb(0);

        public static Color Blue => FromArgb(0);

        public static Color Yellow => FromArgb(0);

        public int ToArgb()
        {
            return 0;
        }

        public static Color FromArgb(int argb)
        {
            return new Color();
        }
    }
}

namespace System.ComponentModel
{
    // FIXME: will be removed when netstandard 2.0 is ready
    [AttributeUsage(AttributeTargets.All)]
    public sealed class BrowsableAttribute : Attribute
    {
        public BrowsableAttribute(bool browsable)
        {
        }
    }

    // FIXME: will be removed when netstandard 2.0 is ready
    [AttributeUsage(AttributeTargets.All)]
    public sealed class PasswordPropertyTextAttribute : Attribute
    {
        public PasswordPropertyTextAttribute()
        { }
        public PasswordPropertyTextAttribute(bool password)
        { }
    }

    // FIXME: will be removed when netstandard 2.0 is ready
    [AttributeUsage(AttributeTargets.All)]
    public sealed class ReadOnlyAttribute : Attribute
    {
        public ReadOnlyAttribute(bool isReadOnly)
        { }
    }

    // FIXME: will be removed when netstandard 2.0 is ready
    [AttributeUsage(AttributeTargets.All)]
    public class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute()
        {
        }
        public DescriptionAttribute(string description)
        {
        }
    }

    // FIXME: will be removed when netstandard 2.0 is ready
    [AttributeUsage(AttributeTargets.All)]
    public class CategoryAttribute : Attribute
    {
        public CategoryAttribute()
        {

        }
        public CategoryAttribute(string category)
        {
        }
    }

    public static class Extensions {
        public static bool IsValid(this TypeConverter converter, object value) { return false; }
    }
}
#endif