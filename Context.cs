using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Linq;

namespace MRE.EFCore5.HasDbFunction
{
    public class Context : DbContext
    {
        public Context(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public DbSet<Foo> Foos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var methodInfo = typeof(Extensions).GetMethod(nameof(Extensions.ToTimeZone));

            static string DelimitIdentifier(string identifier) => $"[{identifier.Replace("]", "]]")}]";

            modelBuilder
                .HasDbFunction(methodInfo)
                .HasTranslation(x =>
                {
                    var columnExpression = x.First() as ColumnExpression;

                    var timeZoneExpression = x.Skip(1).First() as SqlConstantExpression;

                    var columnIdentifier = $"{DelimitIdentifier(columnExpression.Table.Alias)}.{DelimitIdentifier(columnExpression.Name)}";

                    var expression = new SqlFragmentExpression($"{columnIdentifier} AT TIME ZONE {timeZoneExpression.TypeMapping.GenerateSqlLiteral(timeZoneExpression.Value)}");

                    Console.WriteLine($"Translation => {expression.Sql}");

                    return expression;
                });
        }
    }
}
