// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms
{
  using System.Collections.Generic;
  using System.Threading;

  public class ConsumerQueue<T>
  {
    private readonly object locker = new object();
    private readonly Queue<T> queue;
    private readonly EventWaitHandle newItems;

    public EventWaitHandle NewItems
    {
      get { return (newItems); }
    }

    public ConsumerQueue()
    {
      queue = new Queue<T>();
      newItems = new ManualResetEvent(false);
    }

    public void Enqueue(T item)
    {
      lock (locker)
      {
        queue.Enqueue(item);
        newItems.Set();
      }
    }

    public bool TryDequeue(out T item, int milisecondsTimeout = -1)
    {
      if (newItems.WaitOne(milisecondsTimeout))
      {
        lock (locker)
        {
          item = queue.Dequeue();

          if (queue.Count == 0)
          {
            newItems.Reset();
          }
        }

        return (true);
      }

      item = default(T);
      return (false);
    }
  }
}