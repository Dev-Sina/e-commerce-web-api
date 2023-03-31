namespace eCommerceWebAPI.Application.Catalogs.Categories.Commands.EditCategory
{
    public class EditCategoryDto
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DisplayOrder { get; set; }

        public long? ParentCategoryId { get; set; }
    }
}
