using FarmaciaMVC.Models;

namespace FarmaciaMVC.Data;

public sealed class DemoDashboardRepository : IDashboardRepository
{
    private static readonly string[] AvatarColors = ["avatar-mint", "avatar-lilac", "avatar-peach", "avatar-blue", "avatar-rose"];

    public Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.Now;
        var sales = new[]
        {
            new SaleSummary("V-1048", "María González", now.AddMinutes(-18), 845.50m, "Completada"),
            new SaleSummary("V-1047", "Carlos Rodríguez", now.AddHours(-1), 1290.00m, "Completada"),
            new SaleSummary("V-1046", "Lucía Martínez", now.AddHours(-2), 375.25m, "Completada"),
            new SaleSummary("V-1045", "José Pérez", now.AddHours(-3), 2180.00m, "Completada"),
            new SaleSummary("V-1044", "Ana Fernández", now.AddHours(-4), 620.75m, "Completada")
        };
        var customers = new[]
        {
            new CustomerSummary("María González", "maria.gonzalez@email.com", "MG", AvatarColors[0], now.AddDays(-1)),
            new CustomerSummary("Carlos Rodríguez", "carlos.rodriguez@email.com", "CR", AvatarColors[1], now.AddDays(-2)),
            new CustomerSummary("Lucía Martínez", "lucia.martinez@email.com", "LM", AvatarColors[2], now.AddDays(-4)),
            new CustomerSummary("José Pérez", "jose.perez@email.com", "JP", AvatarColors[3], now.AddDays(-7)),
            new CustomerSummary("Ana Fernández", "ana.fernandez@email.com", "AF", AvatarColors[4], now.AddDays(-9))
        };

        var model = new DashboardViewModel
        {
            TodaySales = 12480.50m,
            TodayTransactions = 28,
            ActiveCustomers = 186,
            MonthSales = 248650.75m,
            IsDemo = true,
            RecentSales = sales,
            RecentCustomers = customers
        };

        return Task.FromResult(model);
    }
}
