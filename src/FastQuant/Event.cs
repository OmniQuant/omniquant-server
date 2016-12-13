// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;
using FastQuant.Data;

namespace FastQuant
{
    [DataContract]
    public class Event
    {
        [DataMember]
        protected internal DateTime dateTime;

        public virtual byte TypeId => EventType.Event;

        public DateTime DateTime
        {
            get
            {
                return this.dateTime;
            }
            set
            {
                this.dateTime = value;
            }
        }

        public Event()
        {
        }

        public Event(DateTime dateTime)
        {
            DateTime = dateTime;
        }

        public string ToJSON() => JsonSerializer.SerializeToString(this);
        
        public override string ToString() => $"{DateTime} {GetType()}";
    }
}
