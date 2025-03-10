using Microsoft.AspNetCore.Mvc;
using Infrastructure.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.ViewModels;
using Application.Common.Interfaces;

namespace Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IUnitOfWorkRepository _unitOfWork;
        public VillaNumberController(IUnitOfWorkRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //       View
        public IActionResult Index()
        {
            var VillaNumbers = _unitOfWork.VillaNumber.GetAll(includeProperties: "Villa");
            return View(VillaNumbers);
        }

        //       Create
        public IActionResult Create()
        {
            VillaNumberVM VillaNumberVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(VillaNumberVM);
        }

        [HttpPost]
        public IActionResult Create(VillaNumberVM obj)
        {
            bool roomNumberExists = _unitOfWork.VillaNumber.Any(u => u.Villa_Number == obj.VillaNumber.Villa_Number);


            // ModelState.Remove("Villa "); // to remove the validation error or use [ValidateNever] in the model
            if (ModelState.IsValid && !roomNumberExists) 
            {
                _unitOfWork.VillaNumber.Add(obj.VillaNumber);
                _unitOfWork.Save();
                TempData["success"] = "Villa Number Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            if (roomNumberExists)
            {
                TempData["error"] = "Villa Number already exists";
            };
            obj.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(obj);
        }

            
        //         Update
        public IActionResult Update(int villaNumberId) 
        {
            VillaNumberVM VillaNumberVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _unitOfWork.VillaNumber.Get(u => u.Villa_Number== villaNumberId)
            };
            if (VillaNumberVM.VillaNumber == null) 
            { 
                return RedirectToAction("Error", "Home"); 
             }
            return View(VillaNumberVM);
        }
        [HttpPost]
        public IActionResult Update(VillaNumberVM villaNumberVM)
        {

            // ModelState.Remove("Villa "); // to remove the validation error or use [ValidateNever] in the model
            if (ModelState.IsValid)
            {
                _unitOfWork.VillaNumber.Update(villaNumberVM.VillaNumber);
                _unitOfWork.Save();
                TempData["success"] = "Villa Number Updated Successfully";
                return RedirectToAction(nameof(Index));
            }

            villaNumberVM.VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(villaNumberVM);
            }


        //           Delete
        public IActionResult Delete(int villaNumberId)
        {
            VillaNumberVM VillaNumberVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                VillaNumber = _unitOfWork.VillaNumber.Get(u => u.Villa_Number == villaNumberId)
            };
            if (VillaNumberVM.VillaNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(VillaNumberVM);
        }
        [HttpPost]
        public IActionResult Delete(VillaNumberVM villaNumberVM)
        {
            VillaNumber? objFormDb =_unitOfWork.VillaNumber
                .Get(u => u.Villa_Number == villaNumberVM.VillaNumber.Villa_Number);
            if (objFormDb is not null)
            {
                _unitOfWork.VillaNumber.Remove(objFormDb);
                _unitOfWork.Save();
                TempData["success"] = "Villa Number Deleted Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Villa Number could not deleted.";
            return View();

        }
    }
}
