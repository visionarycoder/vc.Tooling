namespace VisionaryCoder.Generators.Common.Metadata;

public static class AttributeReader
{
    public static IEnumerable<AttributeData> GetAttributes(ISymbol symbol)
    {
        return symbol.GetAttributes();
    }
}