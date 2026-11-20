using FarmaciaMVC.Models;
using Npgsql;

namespace FarmaciaMVC.Data;

public sealed class PostgresDashboardRepository(NpgsqlDataSource dataSource) : IDashboardRepository
{
    public async Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

        const string metricsSql = """
            SELECT
                COALESCE((SELECT SUM(total_amount) FROM sales
                          WHERE status = 'Completada' AND sold_at >= CURRENT_DATE), 0),
                (SELECT COUNT(*) FROM sales
                 WHERE status = 'Completada' AND sold_at >= CURRENT_DATE),
                (SELECT COUNT(*) FROM customers WHERE is_active),
                COALESCE((SELECT SUM(total_amount) FROM sales
                          WHERE status = 'Completada'
                            AND sold_at >= date_trunc('month', CURRENT_DATE)), 0);
            """;

        decimal todaySales;
        int todayTransactions;
        int activeCustomers;
        decimal monthSales;

        await using (var command = new NpgsqlCommand(metricsSql, connection))
        await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            if (!await reader.ReadAsync(cancellationToken))
            {
                throw new InvalidOperationException("No se pudieron leer las métricas del panel.");
            }

            todaySales = reader.GetDecimal(0);
            todayTransactions = reader.GetInt32(1);
            activeCustomers = reader.GetInt32(2);
            monthSales = reader.GetDecimal(3);
        }

        var sales = new List<SaleSummary>();
        const string salesSql = """
            SELECT s.reference, COALESCE(c.full_name, 'Cliente ocasional'),
                   s.sold_at, s.total_amount, s.status
            FROM sales s
            LEFT JOIN customers c ON c.id = s.customer_id
            ORDER BY s.sold_at DESC
            LIMIT 8;
            """;

        await using (var command = new NpgsqlCommand(salesSql, connection))
        await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                sales.Add(new SaleSummary(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetFieldValue<DateTimeOffset>(2),
                    reader.GetDecimal(3),
                    reader.GetString(4)));
            }
        }

        var customers = new List<CustomerSummary>();
        const string customersSql = """
            SELECT c.full_name, c.email, c.created_at, MAX(s.sold_at)
            FROM customers c
            LEFT JOIN sales s ON s.customer_id = c.id AND s.status = 'Completada'
            WHERE c.is_active
            GROUP BY c.id, c.full_name, c.email, c.created_at
            ORDER BY MAX(s.sold_at) DESC NULLS LAST, c.created_at DESC
            LIMIT 8;
            """;

        await using (var command = new NpgsqlCommand(customersSql, connection))
        await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            var customerIndex = 0;
            while (await reader.ReadAsync(cancellationToken))
            {
                var name = reader.GetString(0);
                var initials = string.Join("", name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Take(2)
                    .Select(part => char.ToUpperInvariant(part[0])));
                DateTimeOffset? lastPurchase = reader.IsDBNull(3)
                    ? null
                    : reader.GetFieldValue<DateTimeOffset>(3);

                customers.Add(new CustomerSummary(
                    name,
                    reader.IsDBNull(1) ? "—" : reader.GetString(1),
                    initials,
                    $"avatar-{new[] { "mint", "lilac", "peach", "blue", "rose" }[customerIndex++ % 5]}",
                    lastPurchase));
            }
        }

        return new DashboardViewModel
        {
            TodaySales = todaySales,
            TodayTransactions = todayTransactions,
            ActiveCustomers = activeCustomers,
            MonthSales = monthSales,
            IsDemo = false,
            RecentSales = sales,
            RecentCustomers = customers
        };
    }
}
