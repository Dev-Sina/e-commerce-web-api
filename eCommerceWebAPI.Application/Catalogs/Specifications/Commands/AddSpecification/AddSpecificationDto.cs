namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.AddSpecification
{
    public class AddSpecificationDto
    {
        public string Name { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }
    }
}
