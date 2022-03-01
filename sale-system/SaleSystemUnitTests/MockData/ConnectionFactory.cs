using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SaleSystem.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleSystemUnitTests.MockData
{
    class ConnectionFactory : IDisposable
    {
        private SqliteConnection _connection;
        public SaleSystemDBContext CreateContextForSQLite()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var option = new DbContextOptionsBuilder<SaleSystemDBContext>()
                             .UseSqlite(_connection)
                             .Options;

            var context = new SaleSystemDBContext(option);

            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            return context;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
