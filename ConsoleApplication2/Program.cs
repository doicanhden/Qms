using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Vido;
namespace ConsoleApplication2
{
  class Program
  {
    static void Main(string[] args)
    {
      var format = "IMG_{EntryTime:HHmmss}{EntryGate}{Direction}_{UniqueId}_{UserData}{ImgIndex}.jpg";
      var formatArgs = new
      {
        EntryTime = DateTime.Now,
        EntryGate = "G1",
        Direction = "IM",
        UniqueId = "1234569",
        UserData = "59S121893",
        ImgIndex = 0
      };

      Console.WriteLine(format.NamedFormat(formatArgs));
      Console.ReadKey();
    }

  }
}
