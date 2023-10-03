﻿using Microsoft.EntityFrameworkCore;

namespace ScheduleDAL.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Building
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<Audience> Audiences { get; set; }
    }
}
