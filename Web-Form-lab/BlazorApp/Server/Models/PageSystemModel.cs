namespace Whosales.Web.Models
{
    public record PageSystemModel
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int PageCount { get; set; } = 1;
    }
}
