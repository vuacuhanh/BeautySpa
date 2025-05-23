﻿using BeautySpa.Contract.Repositories.Entity;
using BeautySpa.Core.Base;

public class WorkingHour : BaseEntity
{
    public int DayOfWeek { get; set; }
    public TimeSpan OpeningTime { get; set; }
    public TimeSpan ClosingTime { get; set; }
    public bool IsWorking { get; set; } = true;

    public Guid ServiceProviderId { get; set; }
    public virtual ServiceProvider? ServiceProvider { get; set; }
}
