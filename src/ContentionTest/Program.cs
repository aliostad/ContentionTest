using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace ContentionTest
{
    class Program
    {
        static int total = 0;

        static void Main(string[] args)
        {
            bool async = false;
            const int n = 100000;

            ThreadPool.SetMaxThreads(50, 100);
            ThreadPool.SetMinThreads(50, 100);

            byte[] buffer = new byte[10000 * 1024];
            Random random = new Random();
            random.NextBytes(buffer);

            int thcount = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < n; i++)
            {
                //Thread.Sleep(10);
                if (!async)
                {
                    Console.Write("\r" + i);                    
                }
                var writer = new ContentionWriter(10*1024);
                
                if(async)
                    writer.WriteAsync(buffer)
                        .ContinueWith((t) =>
                        {
                            writer.Stopwatch.Stop();
                            Console.Write("\r" + writer.Stopwatch.Elapsed + "(" + i + ") ");
                            writer.Dispose();
                            Interlocked.Increment(ref total);
                        });
                else
                {
                    writer.Write(buffer);   
                    writer.Dispose();
                }
            }


            if(async)
            {
                while (total < n - 1)
                {
                    Thread.Sleep(200);
                }
            }

         


            stopwatch.Stop();
            Console.WriteLine();            
            Console.WriteLine(stopwatch.Elapsed);

            Console.Read();
        }
    }
}
