using System.Text;

namespace BeautySpa.Core.Utils
{
    public class CoreHelper
    {
        public static DateTimeOffset SystemTimeNow => TimeHelper.ConvertToUtcPlus7(DateTimeOffset.Now);

        public static string GenerateRandomPassword(int length)
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string all = upper + lower + digits;

            var rnd = new Random();
            var res = new StringBuilder();

            // Đảm bảo có ít nhất 1 ký tự mỗi loại
            res.Append(upper[rnd.Next(upper.Length)]);
            res.Append(lower[rnd.Next(lower.Length)]);
            res.Append(digits[rnd.Next(digits.Length)]);

            for (int i = 3; i < length; i++)
                res.Append(all[rnd.Next(all.Length)]);

            return new string(res.ToString().OrderBy(c => rnd.Next()).ToArray());
        }
    }
}
