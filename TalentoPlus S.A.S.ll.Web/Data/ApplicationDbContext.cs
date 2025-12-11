using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalentoPlus_S.A.S.ll.Web.Data.Entities;

namespace TalentoPlus_S.A.S.ll.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Configure DateTime properties to use timestamp without timezone for PostgreSQL
            builder.Entity<Employee>()
                .Property(e => e.BirthDate)
                .HasColumnType("timestamp without time zone");
            
            builder.Entity<Employee>()
                .Property(e => e.HireDate)
                .HasColumnType("timestamp without time zone");
            
            // Relationship configuration
            builder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Seed departments based on Excel
            builder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Logística" },
                new Department { Id = 2, Name = "Marketing" },
                new Department { Id = 3, Name = "Recursos Humanos" },
                new Department { Id = 4, Name = "Operaciones" },
                new Department { Id = 5, Name = "Ventas" },
                new Department { Id = 6, Name = "Tecnología" },
                new Department { Id = 7, Name = "Contabilidad" }
            );
        }
    }
}

