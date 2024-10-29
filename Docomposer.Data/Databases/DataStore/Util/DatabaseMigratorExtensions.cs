using FluentMigrator.Builders.Create.Table;

namespace Docomposer.Data.Databases.DataStore.Util
{
    public static class DatabaseMigratorExtensions
    {
        public static ICreateTableColumnOptionOrWithColumnSyntax AsText(this ICreateTableColumnAsTypeSyntax createTableColumnAsTypeSyntax)
        {
            return createTableColumnAsTypeSyntax.AsString(int.MaxValue);
        }
    }
}