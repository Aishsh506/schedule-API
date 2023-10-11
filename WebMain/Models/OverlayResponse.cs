using Common.Enums;
using Common.Exceptions;

namespace Web.Models
{
    public class OverlayResponse
    {
        public OverlayResponse(DataConflictException ex)
        {
            OverlayedItem = ex.Cause;
            LessonId = ex.ObjectId;
        }
        public ScheduleItems? OverlayedItem { get; set; }
        public Guid? LessonId { get; set; }
    }
}
