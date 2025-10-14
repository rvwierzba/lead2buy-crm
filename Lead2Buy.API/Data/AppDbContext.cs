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
        public DbSet<FunnelStage> FunnelStages { get; set; }
        public DbSet<StageResponsibility> StageResponsibilities { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            

           // Extensão necessária para gen_random_uuid()
            modelBuilder.HasPostgresExtension("pgcrypto");

            // Users
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            // Contacts
            modelBuilder.Entity<Contact>()
                .Property(c => c.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            // CrmTasks
            modelBuilder.Entity<CrmTask>()
                .Property(t => t.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            // Interactions
            modelBuilder.Entity<Interaction>()
                .Property(i => i.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            // NetworkContacts
            modelBuilder.Entity<NetworkContact>()
                .Property(n => n.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            // ChatJobs
            modelBuilder.Entity<ChatJob>()
                .Property(j => j.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            // FunnelStages
            modelBuilder.Entity<FunnelStage>()
                .Property(f => f.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            // StageResponsibilities
            modelBuilder.Entity<StageResponsibility>()
                .Property(s => s.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            // UserActivities (histórico de ações do usuário)
            modelBuilder.Entity<UserActivity>(b =>
            {
                b.ToTable("UserActivities");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
                b.Property(x => x.OccurredAt).IsRequired();
                b.Property(x => x.ActionType).HasMaxLength(50).IsRequired();
                b.Property(x => x.Details).HasMaxLength(500);

                b.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.Contact)
                    .WithMany()
                    .HasForeignKey(x => x.ContactId)
                    .OnDelete(DeleteBehavior.Cascade);
            }); 


        }




    }
}