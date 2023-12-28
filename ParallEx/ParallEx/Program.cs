namespace ParallEx
{
    using System;
    using System.Threading;

    class Program
    {
        static Semaphore semaphore = new Semaphore(3, 3);

        static void Main()
        {
            //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            //CancellationToken token = cancellationTokenSource.Token;

            int successExamCount = 0;
            int st = 0;

            for (int i = 0; i < 25; i++)
            {
                Thread student = new Thread(() =>
                {
                    int id = Interlocked.Increment(ref st);
                    //while (!token.IsCancellationRequested)
                    //{
                        semaphore.WaitOne(/*TimeSpan.FromSeconds(9), true*/);

                        //авария
                        if (new Random().Next(10) < 1)
                        {
                            Console.WriteLine("Мы потеряли его (↓_↓)");
                            return;
                        }

                        Thread.Sleep(1000);
                        semaphore.Release();

                        if (new Random().Next(2) == 0)
                        {
                            Console.WriteLine("Студент " + id + " сдал экзамен");
                            Interlocked.Increment(ref successExamCount);
                        }
                        else Console.WriteLine("Студент " + id + " не сдал");
                    //}
                });

                student.Start();
            }

            Thread.Sleep(10000);

            //cancellationTokenSource.Cancel();

            Console.WriteLine(" число сдавших: " + successExamCount);
        }
    }
}