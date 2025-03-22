namespace Blog.Web.Models.ViewModels
{
    public class AddLikeRequest
    {
        public Guid BlogId { get; set; }
        public Guid UserId { get; set; }
    }
}
