using System.ComponentModel.DataAnnotations.Schema;
using Grip.DAL.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Grip.DAL;

public class ApplicationDbContext : IdentityDbContext<User, Role, int>
{
    public virtual DbSet<PassiveTag> PassiveTags { get; set; } = null!;
    public virtual DbSet<Attendance> Attendances { get; set; } = null!;
    public virtual DbSet<Group> Groups { get; set; } = null!;
    public virtual DbSet<Class> Classes { get; set; } = null!;
    public virtual DbSet<Station> Stations { get; set; } = null!;
    public virtual DbSet<Exempt> Exempts { get; set; } = null!;

    IConfiguration _configuration;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public ApplicationDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DatabaseConnection"));
            //TODO remove
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<PassiveTag>().HasOne(p => p.User).WithMany(u => u.PassiveTags);
        builder.Entity<Attendance>().HasOne(a => a.User).WithMany(u => u.Attendances);
        builder.Entity<Attendance>().HasOne(a => a.Station).WithMany(s => s.Attendances);
        builder.Entity<Group>().HasMany(g => g.Users).WithMany(u => u.Groups);
        builder.Entity<Group>().HasMany(g => g.Classes).WithOne(c => c.Group);
        builder.Entity<Exempt>().HasOne(e => e.IssuedBy).WithMany(u => u.IssuedExemptions);
        builder.Entity<Exempt>().HasOne(e => e.IssuedTo).WithMany(u => u.Exemptions);
        builder.Entity<Class>().HasOne(c => c.Station).WithMany(s => s.Classes);
    }

    public virtual DbSet<Grip.DAL.Model.Exempt> Exempt { get; set; } = default!;
}
