using Blog.Web.Data;
using Blog.Web.Models.Domain;
using Blog.Web.Models.ViewModels;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Web.Controllers
{
    public class AdminTagsController : Controller
    {
        private readonly ITagRepository tagRepository;

        public AdminTagsController(ITagRepository tagRepository)
        {
            this.tagRepository = tagRepository;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {
            //Mapping AddTagRequest to Tag domain model
            var tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName
            };

            await tagRepository.AddAsync(tag);

            return RedirectToAction("List");
        }

        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List()
        {
            //use dbContext to read the tags
            var tags = await tagRepository.GetAll();

            return View(tags);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var tag = await tagRepository.GetAsync(id);

            if(tag != null)
            {
                var editTagRequest = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName
                };
                return View(editTagRequest);
            }
            return View(null);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest editTag)
        {
            var tag = new Tag
            {
                Id = editTag.Id,
                Name = editTag.Name,
                DisplayName = editTag.DisplayName
            };
            var updatedTag = await tagRepository.UpdateAsync(tag);

            if(updatedTag != null)
            {
                //Show success notification
            }
            else
            {
                //Show error notification
            }

                return RedirectToAction("Edit", new { id = editTag.Id });

        }
        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTag)
        {
            var deletedTag = await tagRepository.DeleteAsync(editTag.Id);

            if(deletedTag != null)
            {
                //Show success notification
                return RedirectToAction("List");
            }
            //Show error notification
            
            return RedirectToAction("Edit", new { id = editTag.Id });

        }
    }
}
