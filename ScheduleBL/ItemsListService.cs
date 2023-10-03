using ScheduleBL.DTO;
using ScheduleDAL;

namespace ScheduleBL
{
    public interface IItemsListService
    {
        List<AudienceDTO> GetBuildingAudiences(Guid BuildingId);
        List<BuildingDTO> GetBuildings();
        List<GroupDTO> GetGroups();
        List<ProfessorDTO> GetProfessors();
        List<SubjectDTO> GetSubjects();
    }
    public class ItemsListService : IItemsListService
    {
        private readonly ScheduleContext _context;
        public ItemsListService(ScheduleContext context)
        {
            _context = context;
        }
        public List<AudienceDTO> GetBuildingAudiences(Guid BuildingId)
        {
            var building = _context.Buildings.FirstOrDefault(x => x.Id == BuildingId);
            if (building == null)
            {
                throw new KeyNotFoundException();
            }

            return _context.Audiences
                .Where(x => x.Building == building)
                .Select(x => new AudienceDTO(x))
                .ToList();
        }
        public List <BuildingDTO> GetBuildings()
        {
            return _context.Buildings
                .Select(x => new BuildingDTO(x))
                .ToList();
        }
        public List<GroupDTO> GetGroups()
        {
            return _context.Groups
                .Select(x => new GroupDTO(x))
                .ToList();
        }
        public List<ProfessorDTO> GetProfessors()
        {
            return _context.Professors
                .Select(x => new ProfessorDTO(x))
                .ToList();
        }
        public List<SubjectDTO> GetSubjects()
        {
            return _context.Subjects
                .Select(x => new SubjectDTO(x))
                .ToList();
        }
    }
}
