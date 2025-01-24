using Ecommerce.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models.Repository;
using Ecommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Wep.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult GetData()
        {
            var products = _unitOfWork.Product.GetAll(includedWord: "Category");
            return Json(new { data = products });
        }

        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM productVM, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var path = Path.Combine(rootPath, @"Images\Products");
                    var ext = Path.GetExtension(file.FileName);

                    using (var filestream = new FileStream(Path.Combine(path, filename + ext), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    productVM.Product.Image = @"Images\Products\" + filename + ext;
                }

                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Complete();
                TempData["Create"] = "Product created successfully..";
                return RedirectToAction("Index");
            }
            return View(productVM);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound("Error entered id..");
            }

            ProductVM productVM = new ProductVM()
            {
                Product = _unitOfWork.Product.GetItem(x => x.Id == id),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var path = Path.Combine(rootPath, @"Images\Products");
                    var ext = Path.GetExtension(file.FileName);

                    if (productVM.Product.Image != null)
                    {
                        var oldImage = Path.Combine(rootPath, productVM.Product.Image.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);
                        }
                    }
                    using (var filestream = new FileStream(Path.Combine(path, filename + ext), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    productVM.Product.Image = @"Images\Products\" + filename + ext;
                }

                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Complete();
                TempData["Update"] = "Product updated successfully..";
                return RedirectToAction("Index");
            }
            return View(productVM.Product);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound("Error entered id..");
            }

            var productFromDb = _unitOfWork.Product.GetItem(x => x.Id == id);

            return View(productFromDb);
        }


        [HttpDelete]
        public IActionResult DeleteProduct(int? id)
        {
            var productFromDb = _unitOfWork.Product.GetItem(x => x.Id == id);
            if (productFromDb == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            _unitOfWork.Product.Remove(productFromDb);
            var oldimg = Path.Combine(_webHostEnvironment.WebRootPath, productFromDb.Image.TrimStart('\\'));
            if (System.IO.File.Exists(oldimg))
            {
                System.IO.File.Delete(oldimg);
            }
            _unitOfWork.Complete();
            return Json(new { success = true, message = "file has been Deleted" });
        }
    }
}
