namespace eCommerceWebAPI.Application.Addresses
{
    public partial class CityDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public long ProvinceId { get; set; }

        public int DisplayOrder { get; set; }

        public virtual ProvinceDto Province { get; set; } = new();
    }
}
