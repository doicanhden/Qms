// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido
{
  using System.Text;

  public class NoopPrinter : IPrinter
  {
    public string Name { get; set; }

    public NoopPrinter()
    {
      this.Name = "Noop Printer";
    }

    public PrintResult Print(byte[] data)
    {
      return (PrintResult.Success);
    }

    public PrintResult Print(byte[] data, Encoding encoding)
    {
      return (PrintResult.Success);
    }
  }
}