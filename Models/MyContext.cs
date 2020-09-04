using Microsoft.EntityFrameworkCore;

namespace loginAndRegistration.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter alongcopy
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> user { get; set; }
    }
}