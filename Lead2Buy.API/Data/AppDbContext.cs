using Microsoft.EntityFrameworkCore;
using Lead2Buy.API.Models; 

namespace Lead2Buy.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Interaction> Interactions { get; set; }
        public DbSet<CrmTask> CrmTasks { get; set; }
        public DbSet<ChatJob> ChatJobs { get; set; }
        public DbSet<NetworkContact> NetworkContacts { get; set; }
    }
}