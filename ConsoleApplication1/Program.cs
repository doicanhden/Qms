namespace ConsoleApplication1
{
  using System;
  using System.Collections.Concurrent;
  using System.Threading.Tasks;

  class Program
  {
    static void Main(string[] args)
    {
      ConcurrentQueue<int> queue = new ConcurrentQueue<int>();

      Task task1 = new Task(() =>
      {
        int top;
        while (queue.TryDequeue(out top))
        {
          Console.WriteLine(top);
        }
      });

      Task task2 = new Task(() =>
      {
        for (int i = 0; i < 200; ++i)
          queue.Enqueue(i);
      });
      Task task3 = new Task(() =>
      {
        for (int i = 0; i < 50; ++i)
          queue.Enqueue(i * 10);
      });
      
      task2.Start();
      task3.Start();
      task1.Start();

      Console.ReadLine();
    }
  }
}
