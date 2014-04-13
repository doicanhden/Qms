// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  internal class TaskState
  {
    private readonly Task task;

    public IGate Gate { get; private set; }
    public ConsumerQueue<EntryArgs> Entries { get; private set; }

    public EventWaitHandle EntryAllow { get; private set; }
    public EventWaitHandle EntryBlock { get; private set; }
    public EventWaitHandle TaskStop { get; private set; }

    public TaskState(IGate gate, Action<TaskState> workerThread)
    {
      this.Gate = gate;
      this.Entries = new ConsumerQueue<EntryArgs>();
      this.EntryAllow = gate.Allow;
      this.EntryBlock = gate.Block;
      this.TaskStop = new ManualResetEvent(false);

      this.task = Task.Factory.StartNew(
        (x) => workerThread(x as TaskState), this);
    }

    public void Close()
    {
      this.TaskStop.Set();
      this.task.Wait();
    }
  }
}