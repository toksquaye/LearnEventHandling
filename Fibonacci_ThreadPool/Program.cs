using System;
using System.Threading;

namespace Fibonacci_ThreadPool
{
    public class Fibonacci
    {
        private int _n;
        private int _fibOfN;
        private ManualResetEvent _doneEvent;

        public int N { get { return _n; } }
        public int FibOfN { get { return _fibOfN; } }

        // Constructor. 
        public Fibonacci(int n, ManualResetEvent doneEvent)
        {
            _n = n;
            _doneEvent = doneEvent;
        }

        // Wrapper method for use with thread pool. 
        public void ThreadPoolCallback(Object threadContext)
        {
            int threadIndex = (int)threadContext;
            Console.WriteLine("thread {0} started...", threadIndex);
            _fibOfN = Calculate(_n);
            Console.WriteLine("thread {0} result calculated...", threadIndex);
            _doneEvent.Set();
        }

        // Recursive method that calculates the Nth Fibonacci number. 
        public int Calculate(int n)
        {
            if (n <= 1)
            {
                return n;
            }

            return Calculate(n - 1) + Calculate(n - 2);
        }
    }

    public class ThreadPoolExample
    {
        static void Main()
        {
            const int FibonacciCalculations = 10;

            // One event is used for each Fibonacci object.
            // ManualResetEvent notifies one or more waiting threads that an event has occured
            ManualResetEvent[] doneEvents = new ManualResetEvent[FibonacciCalculations];
            Fibonacci[] fibArray = new Fibonacci[FibonacciCalculations];
            Random r = new Random(); //random number generator object

            // Configure and start threads using ThreadPool.
            Console.WriteLine("launching {0} tasks...", FibonacciCalculations);
            for (int i = 0; i < FibonacciCalculations; i++)
            {
                doneEvents[i] = new ManualResetEvent(false); //new ManualResetEvent is not signaled
                Fibonacci f = new Fibonacci(r.Next(20, 40), doneEvents[i]); //new Fibonacci object. r.Next generates semi-random 1st parameter. set value of number to calculate

                fibArray[i] = f; //array of fibonacci objects
                ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);//grabs thread from threadpool. pass in ThreadPoolCallback method that does calculation
                                                                      //pass in parameter i to ThreadPoolCallback method
            }

            // Wait for all threads in pool to calculate.
            WaitHandle.WaitAll(doneEvents);
            Console.WriteLine("All calculations are complete.");

            // Display the results. 
            for (int i = 0; i < FibonacciCalculations; i++)
            {
                Fibonacci f = fibArray[i];
                Console.WriteLine("Fibonacci({0}) = {1}", f.N, f.FibOfN);
            }

            Console.ReadKey();
        }
    }
}
