namespace ParallEx
{
    using System;
    using System.Threading;

    class Program
    {
        static Semaphore semaphore = new Semaphore(3, 3);

        static void Main()
        {
            List<Thread> students = new(25);

            int successExamCount = 0;
            int st = 0;

            for (int i = 0; i < 25; i++)
            {
                Thread student = new Thread(() =>
                {
                    int id = Interlocked.Increment(ref st);
                    semaphore.WaitOne();


                    //авария
                    if (new Random().Next(7) < 1)
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
                });
                students.Add(student);
                student.Start();
            }

            foreach (Thread student in students) { student.Join(); }

            Console.WriteLine(" число сдавших: " + successExamCount);
        }
    }
}