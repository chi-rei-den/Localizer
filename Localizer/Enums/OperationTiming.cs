using System;

namespace Localizer
{
    [Flags]
    public enum OperationTiming : byte
    {
        BeforeModCtor = 0b00000001,
        BeforeModLoad = 0b00000010,
        BeforeContentLoad = 0b00000100,
        PostContentLoad = 0b00001000,
        Any = 0b11111111,
    }
}
