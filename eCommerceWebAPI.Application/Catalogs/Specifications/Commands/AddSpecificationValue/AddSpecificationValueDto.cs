namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.AddSpecificationValue
{
    public class AddSpecificationValueDto
    {
        public string Name { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public long SpecificationId { get; set; }
    }
}
