using Ecommerce.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models.Repository;
using Ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Wep.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var categories = _unitOfWork.Category.GetAll();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Complete();
                TempData["Create"] = "Category created successfully..";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound("Error entered id..");
            }

            var categoryFromDb = _unitOfWork.Category.GetItem(x => x.Id == id);

            return View(categoryFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Complete();
                TempData["Update"] = "Category updated successfully..";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound("Error entered id..");
            }

            var categoryFromDb = _unitOfWork.Category.GetItem(x => x.Id == id);

            return View(categoryFromDb);
        }


        [HttpPost]
        public IActionResult DeletePost(int? id)
        {
            var categoryFromDb = _unitOfWork.Category.GetItem(x => x.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(categoryFromDb);
            _unitOfWork.Complete();
            TempData["Delete"] = "Category deleted successfully..";
            return RedirectToAction("Index");
        }
    }
}
