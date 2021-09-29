using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace MRE.EFCore5.HasDbFunction
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = @"server=(localdb)\MSSQLLocalDB;database=test;trusted_connection=true;multipleactiveresultsets=true";

            var options = new DbContextOptionsBuilder<Context>()
                .UseSqlServer(connectionString)
                .Options;

            var context = new Context(options);

            context.Database.EnsureCreated();

            if (!context.Foos.Any())
            {
                context.Foos.Add(new Foo() { Created = DateTimeOffset.UtcNow });

                context.SaveChanges();
            }

            try
            {
                var foos = from f in context.Foos
                           select new
                           {
                               Created = f.Created.ToTimeZone("E. Australia Standard Time")
                           };

                foreach (var f in foos)
                {
                    Console.WriteLine($"Foo => {f.Created}");
                }
            }
            catch (NotImplementedException ex)
            {
                Console.Write($"This shouldn't happen => {ex.Message}");
            }
        }
    }
}
