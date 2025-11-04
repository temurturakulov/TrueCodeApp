using FluentMigrator.Runner.VersionTableInfo;

namespace TrueCodeApp.Migrator;

#pragma warning disable CS0618 // Type or member is obsolete

[VersionTableMetaData]
public class VersionTable : DefaultVersionTableMetaData
{
    public override string TableName => "version_info";
    public override string ColumnName => "version";
    public override string DescriptionColumnName => "description";
    public override string AppliedOnColumnName => "applied_on";
    public override string UniqueIndexName => "uc_version";
}
