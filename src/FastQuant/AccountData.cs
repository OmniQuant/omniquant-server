// Copyright (c) FastQuant Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace FastQuant
{
    public enum AccountDataType
    {
        AccountValue,
        Position,
        Order
    }

    [DataContract]
    public class AccountData : DataObject
    {
        public override byte TypeId => DataObjectType.AccountData;
        [DataMember]
        public AccountDataType Type { get; }
        [DataMember]
        public string Account { get; }
        [DataMember]
        public byte ProviderId { get; }
        [DataMember]
        public byte Route { get; }
        [DataMember]
        public AccountDataFieldList Fields { get; } = new AccountDataFieldList();

        public AccountData(DateTime datetime, AccountDataType type, string account, byte providerId, byte route) : base(datetime)
        {
            Type = type;
            Account = account;
            ProviderId = providerId;
            Route = route;
        }
    }

    public class AccountDataSnapshot
    {
        public AccountDataEntry[] Entries { get; }

        internal AccountDataSnapshot(AccountDataEntry[] entries)
        {
            Entries = entries;
        }
    }

    public class AccountDataEntry
    {
        public string Account { get; }

        public AccountData Values { get; }

        public AccountData[] Positions { get; }

        public AccountData[] Orders { get; }

        internal AccountDataEntry(string account, AccountData values, AccountData[] positions, AccountData[] orders)
        {
            Account = account;
            Values = values;
            Positions = positions;
            Orders = orders;
        }
    }

    public class AccountDataEventArgs : EventArgs
    {
        public AccountData Data { get; }

        public AccountDataEventArgs(AccountData data)
        {
            Data = data;
        }
    }

    public delegate void AccountDataEventHandler(object sender, AccountDataEventArgs args);
}