namespace ScheduleDAL.Entities
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<Lesson> Lessons { get; set; }
    }
}
