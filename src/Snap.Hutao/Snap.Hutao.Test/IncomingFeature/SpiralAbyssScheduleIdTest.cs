using System;

namespace Snap.Hutao.Test.IncomingFeature;

[TestClass]
public class SpiralAbyssScheduleIdTest
{
    private static readonly TimeSpan Utc8 = new(8, 0, 0);

    [TestMethod]
    public void Test()
    {
        Console.WriteLine($"当前第 {GetForDateTimeOffset(DateTimeOffset.Now)} 期");

        DateTimeOffset dateTimeOffset = new(2020, 7, 1, 4, 0, 0, Utc8);
        Console.WriteLine($"2020-07-01 04:00:00 为第 {GetForDateTimeOffset(dateTimeOffset)} 期");
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

        return periodNum;
    }
}