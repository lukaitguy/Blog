using System.Threading.Tasks;
using Blog.Web.Models.Domain;
using Blog.Web.Models.ViewModels;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminBlogPostsController : Controller
    {
        private readonly ITagRepository _tagRepository;
        private readonly IBlogPostRepository _blogPostRepository;
        public AdminBlogPostsController(ITagRepository tagRepository, IBlogPostRepository blogPostRepository)
        {
            _tagRepository = tagRepository;
            _blogPostRepository = blogPostRepository;
        }

        public IBlogPostRepository BlogPostRepository { get; }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            //get tags from repository
            var tags = await _tagRepository.GetAll();
            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }) 
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {
            //map view model to domain model
            var blogPostDomainModel = new BlogPost
            {
                Heading = addBlogPostRequest.Heading,
                PageTitle = addBlogPostRequest.PageTitle,
                Content = addBlogPostRequest.Content,
                ShortDescription = addBlogPostRequest.ShortDescription,
                FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
                UrlHandle = addBlogPostRequest.UrlHandle,
                PublishedDate = addBlogPostRequest.PublishedDate,
                Author = addBlogPostRequest.Author,
                Visible = addBlogPostRequest.Visible,
                
            };
            //map tags from selected tags
            var selectedTags = new List<Tag>();
            foreach(var selectedTagId in addBlogPostRequest.SelectedTags)
            {
                var selectedTagIdAsGuid = Guid.Parse(selectedTagId);
                var existingTag = await _tagRepository.GetAsync(selectedTagIdAsGuid);
                if(existingTag != null)
                {
                    selectedTags.Add(existingTag);
                }
            }
            //Mapping tags back to domain model
            blogPostDomainModel.Tags = selectedTags;

            await _blogPostRepository.AddAsync(blogPostDomainModel);

            return RedirectToAction("Add");
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            //Call the repository
            var blogPosts = await _blogPostRepository.GetAll();

            return View(blogPosts);  
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            // Retrieve the result from the repository
            var blogPost = await _blogPostRepository.GetAsync(id);
            var tags = await _tagRepository.GetAll();

            if (blogPost != null)
            {
                // map the domain model into the view model
                var model = new EditBlogPostRequest
                {
                    Id = blogPost.Id,
                    Heading = blogPost.Heading,
                    PageTitle = blogPost.PageTitle,
                    Content = blogPost.Content,
                    Author = blogPost.Author,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    UrlHandle = blogPost.UrlHandle,
                    ShortDescription = blogPost.ShortDescription,
                    PublishedDate = blogPost.PublishedDate,
                    Visible = blogPost.Visible,
                    Tags = tags.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }),
                    SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray()
                };
                //pass data to view
                return View(model);
            }
            return View(null);
            
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlog)
        {
            //Map view model back to domain model
            var blogPostDomainModel = new BlogPost
            {
                Id = editBlog.Id,
                Heading = editBlog.Heading,
                PageTitle = editBlog.PageTitle,
                Content = editBlog.Content,
                Author = editBlog.Author,
                FeaturedImageUrl = editBlog.FeaturedImageUrl,
                UrlHandle = editBlog.UrlHandle,
                ShortDescription = editBlog.ShortDescription,
                PublishedDate = editBlog.PublishedDate,
                Visible = editBlog.Visible,
            };
            // map tags into domain model
            var selectedTags = new List<Tag>();

            foreach(var tag in editBlog.SelectedTags)
            {
                if(Guid.TryParse(tag, out var tags))
                {
                    var foundTag = await _tagRepository.GetAsync(tags);
                    if (foundTag != null)
                    {
                        selectedTags.Add(foundTag);
                    }
                }
            }

            blogPostDomainModel.Tags = selectedTags;

            //Submit info to repository to update
            var updatedBlog = await _blogPostRepository.UpdateAsync(blogPostDomainModel);
            if(updatedBlog != null)
            {
                //show success notif
                return RedirectToAction("Edit");
            }
            //show err notif
            return RedirectToAction("Edit");


            //redirect to GET
        }

        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            //Talk to repository to delete this blog post and tags
            var deletedBlog =  await _blogPostRepository.DeleteAsync(editBlogPostRequest.Id);
            if (deletedBlog != null)
            {
                //Show success response
                return RedirectToAction("List");
            }

            //display the response
            return RedirectToAction("Edit", new { id = editBlogPostRequest.Id});
        }


    }
}
