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
        public DbSet<SingularLesson> SingularLessons { get; set; }
        public DbSet<WeeklyLesson> WeeklyLessons { get; set; }
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
        }
    }
}