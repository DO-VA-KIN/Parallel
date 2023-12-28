namespace lr2Threads
{

    using System;
    using System.Diagnostics;
    using System.Threading;


    class Program
    {
        static int[] Counts = new int[100];
        static object LockObject = new();

        struct Fragment
        {
            public int i;
            public string[] strs;
        }

        static void Main()
        {
            Stopwatch sw = new();
            sw.Start();

            Thread[] threads = new Thread[2];

            // генерируем случайную строку
            List<string> strings = new(100);
            for (int i = 0; i < 100; i++)
            {
                Random rnd = new();
                char[] randomString = new char[30000];
                for (int j = 0; j < randomString.Length; j++)
                    randomString[j] = (char)rnd.Next('a', 'z' + 1);
                strings.Add(new string(randomString));
            }

            for (int i = 0; i < threads.Length; i++)
            {
                int onThread = 100 / threads.Length;
                string[] strs = new string[onThread];
                Array.Copy(strings.ToArray(), i * onThread, strs, 0, onThread);
                threads[i] = new Thread(Calculate);
                threads[i].Start(new Fragment { i = i * onThread, strs = strs });
            }

            for (int i = 0; i < threads.Length; i++)
                threads[i].Join(); // ждем завершения всех потоков

            sw.Stop();
            Console.WriteLine("время выполнения " + sw.Elapsed + "\tкол-во потоков " + threads.Length);

            for (int i = 0; i < Counts.Length; i++)
            {
                Console.WriteLine("Строка {0}: {1} вхождений 'a'", i + 1, Counts[i]);
            }
        }

        static void Calculate(object? startIndexObj)
        {
            Fragment fragment = (Fragment)startIndexObj;

            string[] strs = fragment.strs;

            int j = 0;
            for (int i = fragment.i; i < fragment.i + strs.Length; i++)
            {
                // считаем количество вхождений 'a' в строке
                int count = strs[j].Count(c => c == 'a');
                j++;

                lock (LockObject)
                {
                    Counts[i] = count;
                }
            }
        }


    }
}


