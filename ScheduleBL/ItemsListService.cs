using Common.DTO;
using ScheduleDAL;

namespace ScheduleBL
{
    public interface IItemsListService
    {
        List<AudienceDTO> GetAudiences();
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
        public List<AudienceDTO> GetAudiences()
        {
            return _context.Audiences
                .Select(x => x.ToDTO())
                .ToList();
        }
        public List<GroupDTO> GetGroups()
        {
            return _context.Groups
                .Select(x => x.ToDTO())
                .ToList();
        }
        public List<ProfessorDTO> GetProfessors()
        {
            return _context.Professors
                .Select(x => x.ToDTO())
                .ToList();
        }
        public List<SubjectDTO> GetSubjects()
        {
            return _context.Subjects
                .Select(x => x.ToDTO())
                .ToList();
        }
    }
}
