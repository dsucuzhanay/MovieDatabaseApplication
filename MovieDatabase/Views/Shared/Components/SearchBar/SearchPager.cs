namespace MovieDatabase.Views.Shared.Components.SearchBar
{
    public class SearchPager
    {
        public string? Action { get; set; }
        public string? Controller { get; set; }
        public string? SearchText { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public int StartRecord { get; set; }
        public int EndRecord { get; set; }

        public SearchPager(
            string action,
            string controller,
            string searchText,
            int totalItems,
            int page,
            int pageSize)
        {
            // search by title
            Action = action;
            Controller = controller;
            SearchText = searchText;

            // pagination
            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            var currentPage = page;
            var startPage = currentPage - 5;
            var endPage = currentPage + 4;

            if (startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }

            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 10)
                    startPage = endPage - 9;
            }

            StartRecord = (currentPage - 1) * pageSize + 1;
            EndRecord = StartRecord - 1 + pageSize;
            if (EndRecord > totalItems)
                EndRecord = totalItems;

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
    }
}
