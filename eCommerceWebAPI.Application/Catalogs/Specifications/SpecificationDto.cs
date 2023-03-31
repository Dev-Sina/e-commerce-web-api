namespace eCommerceWebAPI.Application.Catalogs.Specifications
{
    public partial class SpecificationDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public virtual List<SpecificationValueDto> SpecificationValues { get; set; } = new();
    }
}
