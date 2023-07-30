using Microsoft.EntityFrameworkCore;

namespace Meedu.Entities;

public class MeeduDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<PrivateLessonOffer> PrivateLessonOffers { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<DaySchedule> DaySchedules { get; set; }
    public DbSet<ScheduleTimespan> ScheduleTimespans { get; set; }
    public DbSet<LessonReservation> LessonReservations { get; set; }
    public DbSet<Image> Images { get; set; }

    public MeeduDbContext(DbContextOptions<MeeduDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(x => x.FirstName)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(x => x.Email)
            .IsRequired();

        modelBuilder.Entity<Role>()
            .Property(x => x.Name)
            .IsRequired();
    }
}
