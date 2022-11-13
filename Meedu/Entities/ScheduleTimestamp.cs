namespace Meedu.Entities
{
    public class ScheduleTimespan
    {
        public Guid Id { get; set; }
        public TimeSpan AvailableFrom { get; set; }
        public TimeSpan AvailableTo { get; set; }
        public virtual User ReservedBy { get; set; }
    }
}