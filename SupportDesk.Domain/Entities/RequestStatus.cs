namespace SupportDesk.Domain.Entities
{
    public class RequestStatus
    {
        public int RequestStatusId { get; set; }
        public string Description { get; set; } = string.Empty!;
        public string Abbreviation { get; set; } = string.Empty!;
        public ICollection<Request>? Requests { get; set; }
    }
}
