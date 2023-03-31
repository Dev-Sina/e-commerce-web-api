namespace eCommerceWebAPI.Application.Addresses
{
    public partial class ProvinceDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public long CountryId { get; set; }

        public int DisplayOrder { get; set; }

        public virtual CountryDto Country { get; set; } = new();

        public virtual List<CityDto> Cities { get; set; } = new();
    }
}
