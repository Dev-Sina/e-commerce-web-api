namespace eCommerceWebAPI.Domain
{
    public interface ISoftDelete
    {
        bool Deleted { get; set; }
    }
}
