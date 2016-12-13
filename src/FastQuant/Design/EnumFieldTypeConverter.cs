using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace FastQuant.Design
{
    public sealed class EnumFieldDisplayNameAttribute : DisplayNameAttribute
    {
        public EnumFieldDisplayNameAttribute(string displayName):base(displayName)
        {
        }
    }

    public class EnumFieldTypeConverter : EnumConverter
    {
        public EnumFieldTypeConverter(Type type) : base(type)
        {
            this.bool_0 = false;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value != null)
            {
                this.method_0();
                return this.dictionary_0[value];
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                this.method_0();
                return this.dictionary_1[(string)value];
            }
            return base.ConvertFrom(context, culture, value);
        }

        private void method_0()
        {
            if (this.bool_0) return;
            this.dictionary_0 = new Dictionary<object, string>();
            this.dictionary_1 = new Dictionary<string, object>();
            foreach (var text in Enum.GetNames(EnumType))
            {
                FieldInfo expr_3F = EnumType.GetField(text);
                var array = (EnumFieldDisplayNameAttribute[])expr_3F.GetCustomAttributes(typeof(EnumFieldDisplayNameAttribute), false);
                string text2 = array.Length == 1 ? array[0].DisplayName : text;
                object value = expr_3F.GetValue(null);
                this.dictionary_0.Add(value, text2);
                this.dictionary_1.Add(text2, value);
            }
            this.bool_0 = true;
        }

        private Dictionary<object, string> dictionary_0;

        private Dictionary<string, object> dictionary_1;

        private bool bool_0;
    }
}
