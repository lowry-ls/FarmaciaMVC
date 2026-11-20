using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FarmaciaMVC.Data;
using FarmaciaMVC.Models;

namespace FarmaciaMVC.Controllers;

public class HomeController : Controller
{
    private readonly IDashboardRepository _dashboardRepository;

    public HomeController(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = await _dashboardRepository.GetDashboardAsync(cancellationToken);
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
