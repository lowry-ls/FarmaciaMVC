using FarmaciaMVC.Models;

namespace FarmaciaMVC.Data;

public interface IDashboardRepository
{
    Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default);
}
