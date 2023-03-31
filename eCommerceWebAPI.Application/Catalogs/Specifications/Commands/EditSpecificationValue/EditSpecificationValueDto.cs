namespace eCommerceWebAPI.Application.Catalogs.Specifications.Commands.EditSpecificationValue
{
    public class EditSpecificationValueDto
    {
        public string Name { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public long SpecificationId { get; set; }
    }
}
