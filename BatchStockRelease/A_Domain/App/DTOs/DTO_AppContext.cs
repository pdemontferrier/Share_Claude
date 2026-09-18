
namespace BatchStockRelease.A_Domain.App.DTOs
{
    public class DTO_AppContext
    {
        public int AppId { get; set; }
        public DateTime AppDate { get; set; }
        public DateTime AppDateTime { get; set; }
        public int AppUserId { get; set; }
        public string? AppDeviceUser { get; set; }
        public string? AppDeviceId { get; set; }
        public string? AppDeviceIP { get; set; }
    }
}