using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            // Діапазон дат за останні 30 днів
            DateTime StartDate = DateTime.Today.AddDays(-30);
            DateTime EndDate = DateTime.Today;

            // Отримання транзакцій із фільтром
            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndDate &&
                            (y.Category.Type == "Income" || y.Category.Type == "Expense") &&
                            y.Amount >= 0) // Ігноруємо транзакції з від'ємними сумами
                .ToListAsync();

            // Загальний дохід
            int TotalIncome = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .Sum(j => j.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C0");

            // Загальні витрати
            int TotalExpense = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");

            // Баланс
            int Balance = TotalIncome - TotalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("uk-UA");
            culture.NumberFormat.CurrencySymbol = "₴";
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(culture, "{0:C0}", Balance);

            // Дані для кругової діаграми (витрати за категоріями)
            ViewBag.DoughnutChartData = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
                })
                .OrderByDescending(l => l.amount)
                .ToList();

            // Дані для сплайн-діаграми (дохід і витрати)
            // Дохід
            List<SplineChartData> IncomeSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = Math.Max(0, k.Sum(l => l.Amount)) // Замінюємо від'ємні значення на 0
                })
                .ToList();

            // Витрати
            List<SplineChartData> ExpenseSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = Math.Max(0, k.Sum(l => l.Amount)) // Замінюємо від'ємні значення на 0
                })
                .ToList();

            // Заповнюємо 30 днів навіть без даних
            string[] Last30Days = Enumerable.Range(0, 31)
                .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            // Об'єднуємо всі дані для Income і Expense
            ViewBag.SplineChartData = Last30Days
                .Select(day => new
                {
                    day = day,
                    income = IncomeSummary.FirstOrDefault(i => i.day == day)?.income ?? 0,
                    expense = ExpenseSummary.FirstOrDefault(e => e.day == day)?.expense ?? 0,
                })
                .ToList();



            // Останні транзакції
            ViewBag.RecentTransactions = await _context.Transactions
                .Include(i => i.Category)
                .OrderByDescending(j => j.Date)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }

    public class SplineChartData
    {
        public string day { get; set; }
        public int income { get; set; }
        public int expense { get; set; }
    }
}
