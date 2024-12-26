using TestTask.Data;
using TestTask.Data.Helpers;
using TestTask.Services.Report.InternalModels;

namespace TestTask.Services.Report;

public static class ReportQueryHelper
{
    public const string MostPopularItemsQuery = """
                                                SELECT
                                                    CAST("Year" AS INT) AS "Year",
                                                    "ItemId",
                                                    CAST("MaxPurchasesOneDayOneUser" AS INT) AS "MaxPurchasesOneDayOneUser"
                                                FROM (
                                                         SELECT
                                                             "ItemId",
                                                             "MaxPurchasesOneDayOneUser",
                                                             "Year",
                                                             ROW_NUMBER() OVER (PARTITION BY "Year" ORDER BY "MaxPurchasesOneDayOneUser" DESC) AS row_num
                                                         FROM (
                                                                  SELECT
                                                                      ui."ItemId",
                                                                      CAST(MAX(COUNT(*)) OVER (PARTITION BY ui."ItemId", EXTRACT(YEAR FROM ui."PurchaseDate")) AS INT) AS "MaxPurchasesOneDayOneUser",
                                                                      CAST(EXTRACT(YEAR FROM ui."PurchaseDate") AS INT) AS "Year"
                                                                  FROM "UserItems" AS ui
                                                                  GROUP BY ui."UserId", ui."ItemId", ui."PurchaseDate"
                                                              ) AS deduplicated
                                                         GROUP BY "ItemId", "Year", "MaxPurchasesOneDayOneUser"
                                                     ) AS groupped
                                                WHERE row_num <= 3
                                                ORDER BY "Year", row_num;
                                                """;

    public static List<ReportItem> GetMostPopularItemsReport(TestDbContext dbContext)
    {
        return DataHelper.RawSqlQuery(
            dbContext, 
            MostPopularItemsQuery, 
            reader => new ReportItem((int)reader[0], (int)reader[1], (int)reader[2])
            );
    }
}