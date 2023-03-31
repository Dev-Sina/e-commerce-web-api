namespace eCommerceWebAPI.Application
{
    public class BasePaginationQuery
    {
        private int _pageNumber = 1;
        private int _pageSize = 100;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value <= 0 ? 100 : value;
        }

        public int Skip => (PageNumber - 1) * PageSize;
    }
}
