using Blog.Web.Models.Domain;
using Blog.Web.Models.ViewModels;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Blog.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly IBlogPostLikeRepository blogPostLikeRepository;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IBlogPostCommentRepository blogPostCommentRepository;

        public BlogsController(IBlogPostRepository blogPostRepository, IBlogPostLikeRepository blogPostLikeRepository, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IBlogPostCommentRepository blogPostCommentRepository)
        {
            this.blogPostRepository = blogPostRepository;
            this.blogPostLikeRepository = blogPostLikeRepository;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.blogPostCommentRepository = blogPostCommentRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {
            var isLiked = false;
            var blog = await blogPostRepository.GetByUrlHandleAsync(urlHandle);
            var blogPostLikeViewModel = new BlogDetailsViewModel();

            if (blog != null)
            {
                var totalLikes = await blogPostLikeRepository.GetTotalLikes(blog.Id);
                if (signInManager.IsSignedIn(User))
                {
                    //Get like for this blog for this user
                    var likesForBlog = await blogPostLikeRepository.GetLikesForBlog(blog.Id);

                    var userId = userManager.GetUserId(User);

                    if(userId != null)
                    {
                        var likeFromUser = likesForBlog.FirstOrDefault(x => x.UserId == Guid.Parse(userId));
                        isLiked = likeFromUser != null;
                    }
                }

                //Get comments for blog
                var blogComments = await blogPostCommentRepository.GetCommentsByBlogIdAsync(blog.Id);
                var blogCommentsForView = new List<BlogComment>();

                foreach (var comment in blogComments)
                {
                    var user = await userManager.FindByIdAsync(comment.UserId.ToString());
                    blogCommentsForView.Add(new BlogComment
                    {
                        Description = comment.Description,
                        DateAdded = comment.DateAdded,
                        Username = user.UserName
                    });
                }

                blogPostLikeViewModel = new BlogDetailsViewModel
                {
                    Id = blog.Id,
                    Heading = blog.Heading,
                    PageTitle = blog.PageTitle,
                    Content = blog.Content,
                    ShortDescription = blog.ShortDescription,
                    FeaturedImageUrl = blog.FeaturedImageUrl,
                    UrlHandle = blog.UrlHandle,
                    PublishedDate = blog.PublishedDate,
                    Author = blog.Author,
                    Visible = blog.Visible,
                    Tags = blog.Tags,
                    TotalLikes = totalLikes,
                    Liked = isLiked,
                    Comments = blogCommentsForView
                };

            }

            return View(blogPostLikeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(BlogDetailsViewModel blogDetailsViewModel)
        {
            if (signInManager.IsSignedIn(User))
            {
                var domainModel = new BlogPostComment
                {
                    BlogPostId = blogDetailsViewModel.Id,
                    Description = blogDetailsViewModel.CommentDescription,
                    DateAdded = DateTime.Now,
                    UserId = Guid.Parse(userManager.GetUserId(User))
                };
                await blogPostCommentRepository.AddAsync(domainModel);

                return RedirectToAction("Index", "Blogs", new { urlHandle = blogDetailsViewModel.UrlHandle });
            }
            return View();
        }
    }
}
