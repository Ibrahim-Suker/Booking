using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookingWeb.Models;
using Application.Common.Interfaces;
using Web.ViewModels;

namespace BookingWeb.Controllers;

public class HomeController : Controller
{
    private readonly IUnitOfWorkRepository _unitOfWork;
    public HomeController(IUnitOfWorkRepository unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        HomeVM homeVM = new()
        {
            VillaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity"),
            Nights = 1,
            CheckInDate = DateOnly.FromDateTime(DateTime.Now),
        };
        return View(homeVM);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
