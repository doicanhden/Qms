// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Utilities
{
  using System;

  public static class Requires
  {
    public static void NotNull<T>(T value, string parameterName)
        where T : class
    {
      if (value == null)
        throw new ArgumentNullException(parameterName);
    }

    public static void NotNullOrEmpty(string value, string parameterName)
    {
      Requires.NotNull(value, parameterName);

      if (value.Length == 0)
        throw new ArgumentException(null, parameterName);
    }
  }
}