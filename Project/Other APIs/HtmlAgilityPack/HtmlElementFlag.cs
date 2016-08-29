namespace HtmlAgilityPack
{
    using System;

    [Flags]
    public enum HtmlElementFlag
    {
        CanOverlap = 8,
        CData = 1,
        Closed = 4,
        Empty = 2
    }
}