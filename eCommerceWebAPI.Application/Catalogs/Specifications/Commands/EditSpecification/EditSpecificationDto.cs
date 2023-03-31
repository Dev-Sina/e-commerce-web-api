namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.EditSpecification
{
    public class EditSpecificationDto
    {
        public string Name { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }
    }
}
