namespace FarmaciaMVC.Models;

public sealed class DashboardViewModel
{
    public decimal TodaySales { get; init; }
    public int TodayTransactions { get; init; }
    public int ActiveCustomers { get; init; }
    public decimal MonthSales { get; init; }
    public bool IsDemo { get; init; }
    public IReadOnlyList<SaleSummary> RecentSales { get; init; } = [];
    public IReadOnlyList<CustomerSummary> RecentCustomers { get; init; } = [];
}

public sealed record SaleSummary(
    string Reference,
    string CustomerName,
    DateTimeOffset SoldAt,
    decimal Total,
    string Status);

public sealed record CustomerSummary(
    string Name,
    string Email,
    string Initials,
    string AvatarColor,
    DateTimeOffset? LastPurchase);
