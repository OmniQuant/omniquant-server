// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace FastQuant
{
    public class NewsUrgency
    {
        public const byte Normal = 0;
        public const byte Flash = 1;
        public const byte Background = 2;
    }

    [DataContract]
    public class News : DataObject
    {
        public override byte TypeId => DataObjectType.News;

        [DataMember]
        internal int ProviderId { get; set; }
        [DataMember]
        internal int InstrumentId { get; set; }
        [DataMember]
        internal byte Urgency { get; set; }
        [DataMember]
        internal string Url { get; set; }
        [DataMember]
        internal  string Headline { get; set; }
        [DataMember]
        internal  string Text { get; set; }


        public News()
        {
        }

        public News(DateTime dateTime, int providerId, int instrumentId, byte urgency, string url, string headline, string text)
            : base(dateTime)
        {
            ProviderId = providerId;
            InstrumentId = instrumentId;
            Urgency = urgency;
            Url = url;
            Headline = headline;
            Text = text;
        }

        public override string ToString() => $"{Headline} : {Text}";
    }
}