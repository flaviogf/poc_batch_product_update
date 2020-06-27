namespace BatchProductUpdate.Api.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public decimal? Price { get; set; }

        public int? Quantity { get; set; }

        public bool HasGrid { get; set; }
    }
}
