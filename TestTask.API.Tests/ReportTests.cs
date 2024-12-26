using Bogus;
using Microsoft.EntityFrameworkCore;
using TestTask.API.Controllers;
using TestTask.Data.Entities;

namespace TestTask.API.Tests;

public class ReportTests : BaseTest
{
    protected override async Task SetupBase()
    {
        var userFaker = CreateUserFaker();
        var itemFaker = CreateItemFaker();

        var users = userFaker.Generate(5);
        var items = itemFaker.Generate(5);

        await Context.DbContext.Users.AddRangeAsync(users);
        await Context.DbContext.Items.AddRangeAsync(items);
        await Context.DbContext.SaveChangesAsync();
    }
    
    [Test]
    public async Task GetPopularItemsAsync_ReturnReportForEachYear()
    {
        // Arrange
        await SetUserItems();
    
        // Act
        var popularItemsResponse = await Rait<ReportController>().Call(controller => controller.GetPopularItemsAsync());

        // Assert
        var yearReports = popularItemsResponse?.Reports;
        
        var report2023 = yearReports?.FirstOrDefault(r => r.Year == 2023);
        Assert.IsNotNull(report2023, "Report for year 2023 should be present.");
        
        var report2024 = yearReports?.FirstOrDefault(r => r.Year == 2024);
        Assert.IsNotNull(report2024, "Report for year 2024 should be present.");
    }
    
    [Test]
    public async Task GetPopularItemsAsync_ReturnUniqueItemsForEachYear()
    {
        // Arrange
        await SetUserItems();
    
        // Act
        var popularItemsResponse = await Rait<ReportController>().Call(controller => controller.GetPopularItemsAsync());

        // Assert
        var yearReports = popularItemsResponse?.Reports;
        foreach (var report in yearReports!)
        {
            var differentItemsCount = report.Items.Select(x => x.ItemName).Distinct().Count();
            var itemsCount = report.Items.Count;
            if (itemsCount != differentItemsCount)
            {
                Assert.Fail($"Items must be unique");
            }
        }
    }

    private async Task SetUserItems()
    {
        var userIds = Context.DbContext.Users.Select(u => u.Id).ToList();
        var itemIds = Context.DbContext.Items.Select(i => i.Id).ToList();

        var userItemFaker = CreateUserItemFaker(userIds, itemIds);

        var userItems2023 = GenerateUserItemsForYear(userItemFaker, 2023, 20);
        var userItems2024 = GenerateUserItemsForYear(userItemFaker, 2024, 20);

        await Context.DbContext.UserItems.AddRangeAsync(userItems2023);
        await Context.DbContext.UserItems.AddRangeAsync(userItems2024);
        await Context.DbContext.SaveChangesAsync();
    }
    
    private Faker<User> CreateUserFaker() =>
        new Faker<User>()
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Balance, f => f.Random.Decimal(0, 1000));

    private Faker<Item> CreateItemFaker() =>
        new Faker<Item>()
            .RuleFor(i => i.Name, f => f.Commerce.ProductName())
            .RuleFor(i => i.Cost, f => f.Random.Decimal(1, 100));

    private Faker<UserItem> CreateUserItemFaker(List<int> userIds, List<int> itemIds) =>
        new Faker<UserItem>()
            .RuleFor(ui => ui.UserId, f => f.PickRandom(userIds))
            .RuleFor(ui => ui.ItemId, f => f.PickRandom(itemIds));

    private IEnumerable<UserItem> GenerateUserItemsForYear(Faker<UserItem> userItemFaker, int year, int count)
    {
        var days = new[]
        {
            new DateTime(year, 5, 10).ToUniversalTime(),
            new DateTime(year, 8, 20).ToUniversalTime()
        };
        
        return userItemFaker.Clone()
            .RuleFor(ui => ui.PurchaseDate, f => days[f.Random.Int(0, days.Length - 1)])
            .Generate(count);
    }

}