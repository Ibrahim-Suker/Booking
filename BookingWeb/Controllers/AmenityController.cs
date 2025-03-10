using Microsoft.AspNetCore.Mvc;
using Infrastructure.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.ViewModels;
using Application.Common.Interfaces;

namespace Web.Controllers
{
    public class AmenityController : Controller
    {
        private readonly IUnitOfWorkRepository _unitOfWork;
        public AmenityController(IUnitOfWorkRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // View
        public IActionResult Index()
        {
            var amenities = _unitOfWork.Amenity.GetAll(includeProperties: "Villa");
            return View(amenities);
        }

        // Create
        public IActionResult Create()
        {
            AmenityVM amenityVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(amenityVM );
        }

        [HttpPost]
        public IActionResult Create(AmenityVM obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Amenity.VillaId == 0) // Ensure VillaId is set
                {
                    ModelState.AddModelError("Amenity.VillaId", "Please select a villa.");
                }
                else
                {
                    _unitOfWork.Amenity.Add(obj.Amenity);
                    _unitOfWork.Save();
                    TempData["success"] = "Amenity Created Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }

            obj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(obj);
        }

        // Update
        public IActionResult Update(int amenityId)
        {
            AmenityVM AmenityVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _unitOfWork.Amenity.Get(u => u.Id == amenityId)
            };
            if (AmenityVM.Amenity == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(AmenityVM);
        }

        [HttpPost]
        public IActionResult Update(AmenityVM amenityVM)
        {
            if (ModelState.IsValid)
            {
                if (amenityVM.Amenity.VillaId == 0) // Ensure VillaId is set
                {
                    ModelState.AddModelError("Amenity.VillaId", "Please select a villa.");
                }
                else
                {
                    _unitOfWork.Amenity.Update(amenityVM.Amenity);
                    _unitOfWork.Save();
                    TempData["success"] = "Amenity Updated Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }

            amenityVM.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(amenityVM);
        }

        // Delete
        public IActionResult Delete(int amenityId)
        {
            AmenityVM AmenityVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _unitOfWork.Amenity.Get(u => u.Id == amenityId)
            };
            if (AmenityVM.Amenity == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(AmenityVM);
        }

        [HttpPost]
        public IActionResult Delete(AmenityVM amenityVM)
        {
            if (amenityVM.Amenity == null || amenityVM.Amenity.Id == 0)
            {
                TempData["error"] = "Invalid Amenity ID.";
                return RedirectToAction(nameof(Index));
            }

            Amenity? objFromDb = _unitOfWork.Amenity.Get(u => u.Id == amenityVM.Amenity.Id);
            if (objFromDb != null)
            {
                _unitOfWork.Amenity.Remove(objFromDb);
                _unitOfWork.Save();
                TempData["success"] = "Amenity Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = "Amenity could not be deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
