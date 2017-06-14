﻿using Daves.DeepDateUpdater.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Daves.DeepDateUpdater.Metadata
{
    public class Column
    {
        public Column(object tableId, object name, object columnId, object isNullable, object isIdentity, object isComputed, object isDateType)
            : this((int)tableId, (string)name, (int)columnId, (bool)isNullable, (bool)isIdentity, (bool)isComputed, (bool)isDateType)
        { }

        public Column(int tableId, string name, int columnId, bool isNullable, bool isIdentity = false, bool isComputed = false, bool isDateType = false)
        {
            TableId = tableId;
            Name = name;
            ColumnId = columnId;
            IsNullable = isNullable;
            IsIdentity = isIdentity;
            IsComputed = isComputed;
            IsDateType = isDateType;
        }

        public int TableId { get; }
        public string Name { get; }
        public int ColumnId { get; }
        public bool IsNullable { get; }
        public bool IsIdentity { get; }
        public bool IsComputed { get; }
        public bool IsDateType { get; }
        public Table Table { get; protected set; }

        public virtual void Initialize(IReadOnlyList<Table> tables)
            => Table = tables.Single(t => t.Id == TableId);

        public string LowercaseSpacelessName
            => Name.ToLowercaseSpacelessName();

        public virtual bool IsCopyable
            => !IsIdentity && !IsComputed;

        public override string ToString()
            => $"{Table}: {Name}";
    }
}
