using System.Text;

namespace VisionaryCoder.Generators.Common.Builders;

sealed class SourceBuilder(StringBuilder sb)
{
    public void Line(string text = "")
    {
        sb.AppendLine(value: text);
    }

    public void Raw(string text)
    {
        sb.Append(value: text);
    }

    public override string ToString()
    {
        return sb.ToString();
    }
}