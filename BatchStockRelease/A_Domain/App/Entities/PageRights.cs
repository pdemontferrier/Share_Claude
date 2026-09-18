namespace BatchStockRelease.A_Domain.App.Entities
{
    public class PageRights
    {
        public bool CanAccess { get; set; } = false;
        public bool CanCreate { get; set; } = false;
        public bool CanRead { get; set; } = false;
        public bool CanUpdate { get; set; } = false;
        public bool CanDelete { get; set; } = false;
        public bool CanControl { get; set; } = false;
        public bool CanValidate { get; set; } = false;
        public bool CanSupervise { get; set; } = false;
        public bool CanMonitor { get; set; } = false;
        public bool CanAdmin { get; set; } = false;
    }
}
