namespace PriceTracker.Models
{
    public class ServiceResponse<T>
    {
        public enum ServiceStatus { NotFound, Created, Updated, Deleted, Error }

        public ServiceStatus Status { get; set; }

        public T? Data { get; set; }

        public int CreatedId { get; set; }

        public List<string> Messages { get; set; } = new();
    }

}
