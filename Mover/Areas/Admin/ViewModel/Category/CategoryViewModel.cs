namespace Mover.Areas.Admin.ViewModel.Category
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
        public DateTime CreatedOn { get; set; }
    }
}
