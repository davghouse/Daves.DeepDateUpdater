using Daves.DeepDateUpdater.Helpers;
using Daves.DeepDateUpdater.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Daves.DeepDateUpdater
{
    public partial class ReferenceGraph
    {
        public class Vertex
        {
            protected internal Vertex(ReferenceGraph referenceGraph, Table table)
            {
                ReferenceGraph = referenceGraph;
                Table = table;
            }

            protected ReferenceGraph ReferenceGraph { get; }
            public Table Table { get; }
            public IReadOnlyList<Reference> DependentReferences { get; protected set; }

            protected internal virtual void Initialize()
                => InitializeDependentReferences();

            protected virtual void InitializeDependentReferences()
            {
                var requiredForeignKeys = Table.ChildForeignKeys
                    .Where(k => k.IsEffectivelyRequired)
                    .Where(k => ReferenceGraph.Tables.Contains(k.ReferencedTable));

                foreach (var foreignKey in requiredForeignKeys)
                {
                    if (!foreignKey.ReferencedTable.HasIdentityColumnAsPrimaryKey)
                        throw new ArgumentException($"As a table with dependent tables, {foreignKey.ReferencedTable} needs an identity column as its primary key.");

                    if (!foreignKey.IsReferencingPrimaryKey)
                        throw new ArgumentException($"As a dependent of {foreignKey.ReferencedTable}, {Table} can have required foreign keys only to that table's primary key.");
                }

                DependentReferences = BuildReferences(requiredForeignKeys);
            }

            protected virtual IReadOnlyList<Reference> BuildReferences(IEnumerable<ForeignKey> foreignKeysReferencingIdentityPrimaryKeys)
            {
                // The foreign keys are likely already equivalent to distinct (fromColumn, toTable) pairs, but need to be sure in case of weird or misconfigured databases.
                var references = foreignKeysReferencingIdentityPrimaryKeys
                    .SelectMany(k => k.ForeignKeyColumns)
                    .Where(fkc => fkc.ReferencedColumn == fkc.ReferencedTable.PrimaryKey.Column)
                    .Select(fkc => new { fkc.ParentColumn, fkc.ReferencedTable })
                    .Distinct()
                    .Select(a => new Reference(this, a.ParentColumn, a.ReferencedTable))
                    .ToReadOnlyList();

                // And some last edge case handling, again probably just for misconfigured databases.
                var columnsDependentOnMultiplePrimaryKeys = references
                    .GroupBy(d => d.ParentColumn)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);
                if (columnsDependentOnMultiplePrimaryKeys.Any())
                    throw new ArgumentException($"{columnsDependentOnMultiplePrimaryKeys.First()} is dependent on multiple primary keys.");

                return references;
            }

            public virtual bool IsDependentlyReferenced()
                => ReferenceGraph.Any(v =>
                    v.DependentReferences.Any(r => r.ReferencedTable == Table));
        }
    }
}
