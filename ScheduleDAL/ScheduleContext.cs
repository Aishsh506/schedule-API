using Microsoft.EntityFrameworkCore;
using ScheduleDAL.Enums;
using ScheduleDAL.Entities;

namespace ScheduleDAL
{
    public class ScheduleContext : DbContext
    {
        public ScheduleContext(DbContextOptions<ScheduleContext> options) : base(options)
        {
        }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Audience> Audiences { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<LessonGroup> LessonGroups { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Building>()
                .HasMany(e => e.Audiences)
                .WithOne(e => e.Building);
            modelBuilder.Entity<Lesson>()
                .HasMany(e => e.Groups)
                .WithMany(e => e.Lessons)
                .UsingEntity<LessonGroup>();

            modelBuilder.Entity<Lesson>()
                .Property(e => e.LessonType)
                .HasDefaultValue(LessonType.Other);

            modelBuilder.Entity<Lesson>()
                .ToTable(t => t.HasCheckConstraint("Timeslot", "Timeslot > 0 AND Timeslot < 8"))
                .ToTable(t => t.HasCheckConstraint("DayOfWeek", "DayOfWeek > 0 AND DayOfWeek < 6"))
                .ToTable(t => t.HasCheckConstraint("StartDate", "StartDate <= EndDate"));
        }
    }
}