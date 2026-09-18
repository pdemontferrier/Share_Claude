
namespace BatchStockRelease.A_Domain.GestStock.DTOs
{
    public class DTO_UserSessionDetails
    {
        public int Id { get; set; }
        public int IdApplication { get; set; }
        public string ApplicationName { get; set; } = string.Empty;
        public bool Accessible { get; set; }
        public int IdUser { get; set; }
        public string DeviceUser { get; set; } = string.Empty;
        public string FullnameUser { get; set; } = string.Empty;
        public string Initial { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceIp { get; set; } = string.Empty;
        public bool Connected { get; set; }
        public DateTime ConnectionDate { get; set; }
        public DateTime? DisconnectionDate { get; set; }
    }
}
