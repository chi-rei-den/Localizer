namespace Localizer
{
    public enum OperationTiming : byte
    {
        BeforeModLoad         = 0b00000001,
        BeforeContentLoad     = 0b00000010,
        PostContentLoad       = 0b00000100,
        Any                   = 0b11111111,
    }
}
