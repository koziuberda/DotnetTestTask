using Microsoft.EntityFrameworkCore;
using TestTask.Data;
using TestTask.Data.Entities;

namespace TestTask.Services;

public class MarketService
{
    private readonly TestDbContext _testDbContext;

    public MarketService(TestDbContext testDbContext)
    {
        _testDbContext = testDbContext;
    }

    public async Task BuyAsync(int userId, int itemId)
    {
        var user = await _testDbContext.Users.FirstOrDefaultAsync(n => n.Id == userId);
        if (user == null)
            throw new Exception("User not found");
        var item = await _testDbContext.Items.FirstOrDefaultAsync(n => n.Id == itemId);
        if (item == null)
            throw new Exception("Item not found");

        if (user.Balance < item.Cost)
        {
            if (item == null)
                throw new Exception("Not enough balance");
        }

        user.Balance -= item.Cost;
        await _testDbContext.UserItems.AddAsync(new UserItem
        {
            UserId = userId,
            ItemId = itemId
        });

        await _testDbContext.SaveChangesAsync();
    }
}