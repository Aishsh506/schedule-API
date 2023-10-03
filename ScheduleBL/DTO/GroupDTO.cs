using ScheduleDAL.Entities;

namespace ScheduleBL.DTO
{
    public class GroupDTO
    {
        public GroupDTO(Group group)
        {
            Id = group.Id;
            Name = group.Name;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
