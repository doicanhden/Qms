namespace Vido
{
  using System;
  using System.IO;
  using System.Text;

  public static class StringExtensionsNamedFormat
  {
    private enum State
    {
      OutsideExpression,
      OnOpenBracket,
      InsideExpression,
      OnCloseBracket,
      End
    }

    private static void OutExpression(StringBuilder sb, string expression, object source)
    {
      var type = source.GetType();

      var format = string.Empty;

      int colonIndex = expression.IndexOf(':');
      if (colonIndex > 0)
      {
        format = expression.Substring(colonIndex + 1);
        expression = expression.Substring(0, colonIndex);
      }

      var prop = type.GetProperty(expression);
      if (prop == null)
      {
        throw new ArgumentException("Not found: " + expression, "pattern");
      }

      if (string.IsNullOrEmpty(format))
      {
        sb.Append(prop.GetValue(source, null));
      }
      else
      {
        sb.AppendFormat("{0:" + format + "}", prop.GetValue(source, null));
      }
    }

    public static string NamedFormat(this string format, object source)
    {
      if (format == null)
      {
        throw new ArgumentNullException("format");
      }

      StringBuilder result = new StringBuilder(format.Length * 2);

      using (var reader = new StringReader(format))
      {
        StringBuilder expression = new StringBuilder();
        int @char = -1;

        State state = State.OutsideExpression;
        do
        {
          switch (state)
          {
            case State.OutsideExpression:
              @char = reader.Read();
              switch (@char)
              {
                case -1:
                  state = State.End;
                  break;
                case '{':
                  state = State.OnOpenBracket;
                  break;
                case '}':
                  state = State.OnCloseBracket;
                  break;
                default:
                  result.Append((char)@char);
                  break;
              }
              break;
            case State.OnOpenBracket:
              @char = reader.Read();
              switch (@char)
              {
                case -1:
                  throw new FormatException();
                case '{':
                  result.Append('{');
                  state = State.OutsideExpression;
                  break;
                default:
                  expression.Append((char)@char);
                  state = State.InsideExpression;
                  break;
              }
              break;
            case State.InsideExpression:
              @char = reader.Read();
              switch (@char)
              {
                case -1:
                  throw new FormatException();
                case '}':
                  OutExpression(result, expression.ToString(), source);
                  expression.Length = 0;
                  state = State.OutsideExpression;
                  break;
                default:
                  expression.Append((char)@char);
                  break;
              }
              break;
            case State.OnCloseBracket:
              @char = reader.Read();
              switch (@char)
              {
                case '}':
                  result.Append('}');
                  state = State.OutsideExpression;
                  break;
                default:
                  throw new FormatException();
              }
              break;
            default:
              throw new InvalidOperationException("Invalid state.");
          }
        } while (state != State.End);
      }

      return (result.ToString());
    }
  }
}