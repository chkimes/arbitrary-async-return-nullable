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

            var list = DoListThings();
            foreach(var item in list)
                Console.WriteLine(item); // 1, 2, 3, 4
            Console.ReadLine();
        }

        static async ListTaskLike<int> DoListThings()
        {
            await ListTaskLike.Yield(1);
            await ListTaskLike.Yield(2);
            await ListTaskLike.Yield(3);
            return 4;
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
