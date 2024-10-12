namespace alert_service.DTOs
{
    public class SubscribeDto
    {
        public int? IdEntityDetails { get; set; }
        public string? IdDMUser { get; set; }
        public string? IdDMEntity { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
    }
}
