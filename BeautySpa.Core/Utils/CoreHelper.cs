using System.Text;

namespace BeautySpa.Core.Utils
{
    public class CoreHelper
    {
        public static DateTimeOffset SystemTimeNow => TimeHelper.ConvertToUtcPlus7(DateTimeOffset.Now);
        public static string GenerateRandomPassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var res = new StringBuilder();
            var rnd = new Random();

            for (int i = 0; i < length; i++)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }

            return res.ToString();
        }
    }
}
