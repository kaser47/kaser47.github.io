using System.Data.Entity;
using RecentlyAddedShows.Web.Data.Entities;

namespace RecentlyAddedShows.Web.Data
{
    public class Context : DbContext
    {
        private const string ConnectionString =
            "workstation id=rasDatabase.mssql.somee.com;packet size=4096;user id=kaser47_SQLLogin_1;pwd=87mcm5rsla;data source=rasDatabase.mssql.somee.com;persist security info=False;initial catalog=rasDatabase";
        public Context() : base(ConnectionString)
        {

        }
        
        public DbSet<Show> Shows { get; set; }
    }
}