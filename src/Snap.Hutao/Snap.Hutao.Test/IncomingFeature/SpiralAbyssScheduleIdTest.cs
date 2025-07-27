using System;

namespace Snap.Hutao.Test.IncomingFeature;

[TestClass]
public class SpiralAbyssScheduleIdTest
{
    private static readonly TimeSpan Utc8 = TimeSpan.FromHours(8);
    private static readonly DateTimeOffset AcrobaticsBattleIntroducedTime = new(2024, 7, 1, 4, 0, 0, Utc8);

    [TestMethod]
    public void Test()
    {
        Console.WriteLine($"当前第 {GetForDateTimeOffset(DateTimeOffset.Now)} 期");

        // 2020-07-01 04:00:00 为第 1 期
        // 2024-06-16 04:00:00 为第 96 期
        // 2024-07-01 04:00:00 为第 97 期
        // 2024-07-16 04:00:00 为第 98 期
        // 2024-08-01 04:00:00 为第 99 期
        Console.WriteLine($"2020-07-01 04:00:00 为第 {GetForDateTimeOffset(new(2020, 07, 01, 4, 0, 0, Utc8))} 期");
        Console.WriteLine($"2024-06-16 04:00:00 为第 {GetForDateTimeOffset(new(2024, 06, 16, 4, 0, 0, Utc8))} 期");
        Console.WriteLine($"2024-07-01 04:00:00 为第 {GetForDateTimeOffset(new(2024, 07, 01, 4, 0, 0, Utc8))} 期");
        Console.WriteLine($"2024-07-16 04:00:00 为第 {GetForDateTimeOffset(new(2024, 07, 16, 4, 0, 0, Utc8))} 期");
        Console.WriteLine($"2024-08-01 04:00:00 为第 {GetForDateTimeOffset(new(2024, 08, 01, 4, 0, 0, Utc8))} 期");
        Console.WriteLine($"2024-08-16 04:00:00 为第 {GetForDateTimeOffset(new(2024, 08, 16, 4, 0, 0, Utc8))} 期");
        Console.WriteLine($"2024-09-01 04:00:00 为第 {GetForDateTimeOffset(new(2024, 09, 01, 4, 0, 0, Utc8))} 期");
    }

    public static int GetForDateTimeOffset(DateTimeOffset dateTimeOffset)
    {
        // Force time in UTC+08
        dateTimeOffset = dateTimeOffset.ToOffset(Utc8);

        ((int year, int mouth, int day), (int hour, _), _) = dateTimeOffset;

        // 2020-07-01 04:00:00 为第 1 期
        int periodNum = (((year - 2020) * 12) + (mouth - 6)) * 2;

        // 上半月：1-15 日, 以及 16 日 00:00-04:00
        if (day < 16 || (day == 16 && hour < 4))
        {
            periodNum--;
        }

        // 上个月：1 日 00:00-04:00
        if (day is 1 && hour < 4)
        {
            periodNum--;
        }

        if (dateTimeOffset >= AcrobaticsBattleIntroducedTime)
        {
            // 当超过 96 期时，每一个月一期
            periodNum = (4 * 12 * 2) + ((periodNum - (4 * 12 * 2)) / 2);
        }

        return periodNum;
    }
}