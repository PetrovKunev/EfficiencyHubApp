using System.Collections.Generic;
using EfficiencyHub.Data.Models;

public class AdminDataViewModel
{
    public List<Project> Projects { get; set; } = new List<Project>();
    public List<Assignment> Tasks { get; set; } = new List<Assignment>();
    public List<Reminder> Reminders { get; set; } = new List<Reminder>();
}
