namespace eCommerceWebAPI.Application.Addresses
{
    public partial class CountryDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public virtual List<ProvinceDto> Provinces { get; set; } = new();
    }
}
