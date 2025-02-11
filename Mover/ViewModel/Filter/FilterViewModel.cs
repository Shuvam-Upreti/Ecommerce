namespace Mover.ViewModel.Filter
{
    public class FilterViewModel
    {
        public string? Search { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 30;
    }
}
