using ScheduleDAL.Entities;

namespace ScheduleBL.DTO
{
    public class BuildingDTO
    {
        public BuildingDTO(Building building)
        {
            Id = building.Id;
            Name = building.Name;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
