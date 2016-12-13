using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Runtime.Serialization.Json;

namespace FastQuant.Data
{
    public static class JsonSerializer
    {
        private static readonly IEnumerable<Type> types = typeof(Framework).Assembly.GetTypes().Where(t => t.GetCustomAttribute<DataContractAttribute>() != null);

        public static object Deserialize(Type type, string data)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data ?? string.Empty)))
                return Deserialize(type, stream);
        }

        public static object Deserialize(Type type, Stream stream)
        {
            object result;
            try
            {
                result = new DataContractJsonSerializer(type, types).ReadObject(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(JsonSerializer)}::{nameof(Deserialize)}: {ex.Message}");
                result = null;
            }
            return result;
        }

        public static byte[] SerializeToBytes(object o)
        {
            byte[] result;
            try
            {
                FixDateTimes(o);
                using (var stream = new MemoryStream())
                {
                    new DataContractJsonSerializer(o.GetType(), types).WriteObject(stream, o);
                    result = stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{nameof(JsonSerializer)}::{nameof(SerializeToBytes)}: {ex.Message}");
                result = new byte[0];
            }
            return result;
        }

        public static string SerializeToString(object o) => Encoding.UTF8.GetString(SerializeToBytes(o));

        private static void FixDateTimes(object @object)
        {
            if (@object == null) return;
            var zeroTime = new DateTimeOffset(DateTime.MinValue, TimeSpan.Zero).LocalDateTime;
            var obj = @object is IEnumerable ? (IEnumerable)@object : new[] { @object };
            foreach (var o in obj)
            {
                var fields = o.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(f => f.GetCustomAttribute<DataMemberAttribute>() != null);
                foreach (var f in fields.Where(f => f.FieldType == typeof(DateTime)))
                {
                    var t = (DateTime)f.GetValue(o);
                    if (t < zeroTime)
                        f.SetValue(o, new DateTime(t.Ticks, DateTimeKind.Utc));
                }
                var properties = o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(p => p.GetCustomAttribute<DataMemberAttribute>() != null);
                foreach (var p in properties.Where(p => p.PropertyType == typeof(DateTime)))
                {
                    var t = (DateTime)p.GetValue(o);
                    if (t < zeroTime)
                        p.SetValue(o, new DateTime(t.Ticks, DateTimeKind.Utc));
                }
                foreach (var f in fields.Where(f => f.FieldType.GetCustomAttribute<DataContractAttribute>() != null || f.FieldType.GetInterfaces().Contains(typeof(IEnumerable))))
                {
                }
            }
            throw new NotImplementedException();
        }
    }
}
