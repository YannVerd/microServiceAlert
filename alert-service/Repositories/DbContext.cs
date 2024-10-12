using alert_service.Models;
using Microsoft.EntityFrameworkCore;
using dotenv.net;

namespace alert_service.Repositories
{
     
    public class DBContext(DbContextOptions<DBContext> options) : DbContext(options)
    {
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Chargement des variables d'environnement depuis le fichier .env
            DotEnv.Load();

            // Connection 
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            
            // Chaine de connexion
            var connectionString = $"server={dbHost};port={dbPort};user={dbUser};password={dbPassword};database={dbName};";

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // une notify appartient à une seule alerte
            // une alerte peut avoir plusieurs notify
            // foreign key de IDAlert dans notify
            // obligatoire
            modelBuilder.Entity<Notify>()
                .HasOne(n => n.Alert)  
                .WithMany(a => a.Notifys) 
                .HasForeignKey(n => n.IdAlert)  
                .IsRequired();

            // le modelBuilder expose clairement la relation model/table et empêche la création d'une colonne AlertIdAlert
            modelBuilder.Entity<EntityDetails>(entity =>
            {
                entity.ToTable("entitydetails");
                entity.HasKey(entity => entity.IdEntityDetails);
                entity.Property(entity => entity.IdEntityDetails).HasColumnName("id_entity_details").UseMySqlIdentityColumn();
                entity.Property(entity => entity.IdDMEntity).HasColumnName("id_dm_entity").IsRequired();
                entity.Property(entity => entity.IdAlert).HasColumnName("id_Alert");
                entity.Property(entity => entity.Name).HasColumnName("name").IsRequired().HasMaxLength(50);
                entity.Property(entity => entity.Type).HasColumnName("type").IsRequired().HasMaxLength(50);
                entity.HasOne(a => a.Alert).WithOne(e => e.EntityDetails).HasForeignKey<EntityDetails>(e => e.IdAlert);
                

            });

            modelBuilder.Entity<Alert>()
                .HasMany(n => n.Notifys)
                .WithOne(a => a.Alert);
        
        }

        public DbSet<EntityDetails> EntityDetails {get; set;}
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Notify> Notifys {get; set;}


    }
}
