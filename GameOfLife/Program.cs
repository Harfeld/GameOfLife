using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GameOfLife
{
    class Program
    {
        static void Main(string[] args)
        {
            var gol1 = new SeqGameOfLife(1000);
            var gol2 = new ParGameOfLife(1000);
            var gol3 = new BarGameOfLife(1000);

            Task t1 = Task.Factory.StartNew(() => gol1.Run(10));
            Task.WaitAll(t1);
            System.Console.WriteLine("\n");

            Task t2 = Task.Factory.StartNew(() => gol2.Run(10));
            Task.WaitAll(t2);
            System.Console.WriteLine("\n");

            Task t3 = Task.Factory.StartNew(() => gol3.Run(10));
            Task.WaitAll(t3);
        }
    }
}
