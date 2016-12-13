// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

namespace FastQuant
{
    [DataContract]
    public class GroupEvent : DataObject
    {
        public override byte TypeId => DataObjectType.GroupEvent;

        [DataMember]
        public Group Group { get; set; }

        [DataMember]
        public Event Obj { get; }

        [DataMember]
        public int GroupId { get; }

        public GroupEvent(Event obj, Group group)
        {
            Obj = obj;
            Group = group;
            DateTime = obj.dateTime;
            GroupId = group?.Id ?? -1;
        }

        public GroupEvent(Event obj, int groupId)
        {
            Obj = obj;
            GroupId = groupId;
            DateTime = obj.dateTime;
        }

        public override DataObject Clone()
        {
            return new GroupEvent(Obj, Group);
        }

        public override string ToString() => $"GroupEvent {DateTime}, Id = {GroupId}, {Obj}";
    }

    [DataContract]
    public class Group : DataObject
    {
        private Framework framework;

        private bool clearEvents;

        public override byte TypeId => DataObjectType.Group;

        [DataMember]
        public int Id { get; internal set; }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public Dictionary<string, GroupField> Fields { get; } = new Dictionary<string, GroupField>();

        [DataMember]
        public List<GroupEvent> Events { get; } = new List<GroupEvent>();

        public Framework Framework
        {
            get
            {
                return this.framework;
            }
            set
            {
                this.framework = value;
                if (this.framework != null)
                    this.clearEvents = this.framework.Configuration.IsClearGroupEvents;
            }
        }

        public Group(string name)
        {
            Name = name;
            DateTime = new DateTime(1);
        }

        public void Add(string name, byte type, object value)
        {
            Add(new GroupField(name, type, value));
        }

        public void Add(string name, Color color)
        {
            Add(new GroupField(name, DataObjectType.Color, color));
        }

        public void Add(string name, string value)
        {
            Add(new GroupField(name, DataObjectType.String, value));
        }

        public void Add(string name, int value)
        {
            Add(new GroupField(name, DataObjectType.Int, value));
        }
        public void Add(string name, double value)
        {
            Add(new GroupField(name, DataObjectType.Double, value));
        }
        public void Add(string name, bool value)
        {
            Add(new GroupField(name, DataObjectType.Boolean, value));
        }

        public void Add(string name, DateTime dateTime)
        {
            Add(new GroupField(name, DataObjectType.DateTime, dateTime));
        }

        public void Remove(string fieldName)
        {
            Fields.Remove(fieldName);
        }

        public void OnNewGroupEvent(GroupEvent groupEvent)
        {
            Events.Add(groupEvent);
        }

        private void Add(GroupField groupField,bool bool_0 = true)
        {
            Fields[groupField.Name] = groupField;
            groupField.Group = this;
        }

        public override string ToString() => $"Group {DateTime}, Id = {Id}, Name = {Name}";
    }

    [DataContract]
    public class GroupField
    {
        internal Group Group { get; set; }

        [DataMember]
        private object value;

        [DataMember]
        public string Name { get;  }

        [DataMember]
        public byte Type { get; }

        public object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (this.value == value) return;
                var oldValue = this.value;
                this.value = value;
                if (Group.Id>0)
                Group.Framework?.EventServer.OnLog(new GroupUpdate(Group.Id, Name, Type, this.value, oldValue, GroupUpdateType.FieldUpdated));
            }
        }

        public GroupField(string name, byte type, object value)
        {
            Name = name;
            Type = type;
            this.value = value;
        }
    }

    public enum GroupUpdateType
    {
        FieldAdded,
        FieldRemoved,
        FieldUpdated
    }

    [DataContract]
    public class GroupUpdate : DataObject
    {
        public override byte TypeId => DataObjectType.GroupUpdate;

        public int GroupId { get; }

        public string FieldName { get; }

        public GroupUpdateType UpdateType { get; }

        public byte FieldType { get; }

        public object Value { get; }

        public object OldValue { get; }

        public GroupUpdate(int groupId, string fieldName, byte fieldType, object value, object oldValue, GroupUpdateType updateType)
        {
            GroupId = groupId;
            FieldName = fieldName;
            FieldType = fieldType;
            Value = value;
            OldValue = oldValue;
            UpdateType = updateType;
        }
    }
}