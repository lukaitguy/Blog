using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Blog.Web.Models;
using Blog.Web.Repositories;
using Blog.Web.Models.ViewModels;

namespace Blog.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBlogPostRepository blogPostRepository;
    private readonly ITagRepository tagRepository;

    public HomeController(ILogger<HomeController> logger, IBlogPostRepository blogPostRepository, ITagRepository tagRepository)
    {
        _logger = logger;
        this.blogPostRepository = blogPostRepository;
        this.tagRepository = tagRepository;
    }

    public async Task<IActionResult> Index()
    {
        var blogPosts = await blogPostRepository.GetAll();
        var tags = await tagRepository.GetAll();

        var model = new HomeViewModel
        {
            BlogPosts = blogPosts,
            Tags = tags
        };
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
