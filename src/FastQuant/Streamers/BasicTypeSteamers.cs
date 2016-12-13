﻿// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;
using System.IO;

namespace FastQuant
{
    public class BooleanStreamer : ObjectStreamer
    {
        public BooleanStreamer()
        {
            this.typeId = DataObjectType.Boolean;
            this.type = typeof(bool);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            return reader.ReadBoolean() ? 1 : 0;
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            writer.Write((bool)obj);
        }
    }

    public class ByteStreamer : ObjectStreamer
    {
        public ByteStreamer()
        {
            this.typeId = DataObjectType.Byte;
            this.type = typeof(byte);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            return reader.ReadByte();
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            writer.Write((byte)obj);
        }
    }

    public class CharStreamer : ObjectStreamer
    {
        public CharStreamer()
        {
            this.typeId = DataObjectType.Char;
            this.type = typeof(char);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            return reader.ReadChar();
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            writer.Write((char)obj);
        }
    }

    public class Int16Streamer : ObjectStreamer
    {
        public Int16Streamer()
        {
            this.typeId = DataObjectType.Int16;
            this.type = typeof(short);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            return reader.ReadInt16();
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            writer.Write((short)obj);
        }
    }

    public class Int32Streamer : ObjectStreamer
    {
        public Int32Streamer()
        {
            this.typeId = DataObjectType.Int32;
            this.type = typeof(int);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            return reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            writer.Write((int)obj);
        }
    }

    public class Int64Streamer : ObjectStreamer
    {
        public Int64Streamer()
        {
            this.typeId = DataObjectType.Int64;
            this.type = typeof(long);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            return reader.ReadInt64();
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            writer.Write((long)obj);
        }
    }

    public class DoubleStreamer : ObjectStreamer
    {
        public DoubleStreamer()
        {
            this.typeId = DataObjectType.Double;
            this.type = typeof(double);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            return reader.ReadDouble();
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            writer.Write((double)obj);
        }
    }

    public class ColorStreamer : ObjectStreamer
    {
        public ColorStreamer()
        {
            this.typeId = DataObjectType.Color;
            this.type = typeof(Color);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            return Color.FromArgb(reader.ReadInt32());
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            writer.Write(((Color)obj).ToArgb());
        }
    }

    public class TimeSpanStreamer : ObjectStreamer
    {
        public TimeSpanStreamer()
        {
            this.typeId = DataObjectType.TimeSpan;
            this.type = typeof(TimeSpan);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            return new TimeSpan(reader.ReadInt64());
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            writer.Write(((TimeSpan)obj).Ticks);
        }
    }

    public class DateTimeStreamer : ObjectStreamer
    {
        public DateTimeStreamer()
        {
            this.typeId = DataObjectType.DateTime;
            this.type = typeof(DateTime);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            return new DateTime(reader.ReadInt64());
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            writer.Write(((DateTime)obj).Ticks);
        }
    }

    public class StringStreamer : ObjectStreamer
    {
        public StringStreamer()
        {
            this.typeId = DataObjectType.String;
            this.type = typeof(string);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            return reader.ReadString();
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            writer.Write((string)obj);
        }
    }

    public class ArrayStreamer : ObjectStreamer
    {
        public ArrayStreamer()
        {
            this.typeId = DataObjectType.Array;
            this.type = typeof(object[]);
        }

        public override object Read(BinaryReader reader, byte version)
        {
            int length = reader.ReadInt32();
            var array = new object[length];
            for (int i = 0; i < length; i++)
                array[i] = StreamerManager.Deserialize(reader);
            return array;
        }

        public override void Write(BinaryWriter writer, object obj)
        {
            var array = (object[])obj;
            writer.Write(array.Length);
            for (int i = 0; i < array.Length; i++)
                StreamerManager.Serialize(writer, array[i]);
        }
    }
}