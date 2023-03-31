namespace eCommerceWebAPI.Application.Catalogs.Categories.Commands.AddCategory
{
    public class AddCategoryDto
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DisplayOrder { get; set; }

        public long? ParentCategoryId { get; set; }
    }
}
