using Microsoft.AspNetCore.Mvc;
using Infrastructure.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IUnitOfWorkRepository _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VillaController(IUnitOfWorkRepository unitOfWork , IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        //       View
        public IActionResult Index()
        {
            var villas = _unitOfWork.Villa.GetAll();
            return View(villas);
        }

        //       Create
        public IActionResult Create()
        { 
            return View();
        }

        [HttpPost]
        public IActionResult Create(Villa obj)
        {
            if(obj.Name == obj.Description)
            {
                ModelState.AddModelError("Name", "Name and Description can not be the same");
            }
            if (ModelState.IsValid) 
            {
                if (obj.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString()+Path.GetExtension(obj.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");

                    using var fileSteam = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                    obj.Image.CopyTo(fileSteam);

                    obj.ImageUrl = @"\images\VillaImages\" + fileName;
                }
                else 
                {
                    obj.ImageUrl = "https://placehold.co/600x400";
                }

                _unitOfWork.Villa.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Villa Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

            
        //         Update
        public IActionResult Update(int VillaId) 
        {
            Villa? obj = _unitOfWork.Villa.Get(u => u.Id ==VillaId);
            //Villa? obj = _db.Villas.Find(VillaId);
            //var VillaList = _db.Villas.Where(u => u.Price > 50 && u.Occupancy > 0);
            if (obj == null)
            {
                return RedirectToAction("Error","Home");
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Update(Villa obj)
        {
            if (ModelState.IsValid  &&  obj.Id > 0 )
            {
                if (obj.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(obj.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");

                    if (!string.IsNullOrEmpty(obj.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using var fileSteam = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                    obj.Image.CopyTo(fileSteam);

                    obj.ImageUrl = @"\images\VillaImages\" + fileName;
                }
               
                _unitOfWork.Villa.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Villa Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }


        //           Delete
        public IActionResult Delete(int VillaId)
        {
            Villa? obj = _unitOfWork.Villa.Get(u => u.Id == VillaId);
            if (obj is null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Delete(Villa obj)
        {
            Villa? objFormDb = _unitOfWork.Villa.Get(u => u.Id == obj.Id);
            if (objFormDb is not null)
            {
                if (!string.IsNullOrEmpty(objFormDb.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, objFormDb.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.Villa.Remove(objFormDb);
                _unitOfWork.Save();
                TempData["success"] = "Villa Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
            TempData["error"] = "Villa could not deleted.";
        }
    }
}
