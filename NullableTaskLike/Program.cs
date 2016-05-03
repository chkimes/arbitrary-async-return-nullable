using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullableTaskLike
{
    class Program
    {
        static void Main()
        {
            var nullable = DoNullableThings();
            Console.WriteLine(nullable.HasValue);
            Console.ReadLine();
        }

        static async NullableTaskLike<int> DoNullableThings()
        {
            var val1 = await GetValue();
            var val2 = await GetNoValue();

            var val3 = await GetValue();
            var val4 = await GetValue();
            var val5 = await GetValue();
            var val6 = await GetValue();
            var val7 = await GetValue();
            return val7;
        }

        static async NullableTaskLike<int> GetValue()
        {
            Console.WriteLine("Returning 1");
            return 1;
        }

        static async NullableTaskLike<int> GetNoValue()
        {
            Console.WriteLine("Returning none");
            return await new NullableTaskLike<int>();
        }
    }
}
