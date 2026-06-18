using System.Text;

namespace VisionaryCoder.Generators.Common.Builders;

file sealed class IndentedStringBuilder
{
    private readonly StringBuilder _sb = new();
    private int _indent;
    public void Indent()
    {
        _indent++;
    }

    public void Unindent()
    {
        _indent = Math.Max(val1: 0, val2: _indent - 1);
    }

    public void AppendLine(string text = "")
    {
        if (text.Length > 0)
            _sb.Append(value: ' ', repeatCount: _indent * 4);
        _sb.AppendLine(value: text);
    }
    public override string ToString()
    {
        return _sb.ToString();
    }
}