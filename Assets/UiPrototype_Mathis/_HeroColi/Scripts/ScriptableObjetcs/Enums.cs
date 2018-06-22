using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UIProto.Scriptable.Enums
{
    public enum DataState
    {
        Empty,
        Filled
    }

    public enum BricksType
    {
        Promoter,
        RBS,
        CodingSequence,
        Terminator
    }

    public enum DeviceState
    {
        Empty,
        InCompletion,
        Completed
    }

    public enum PromoterMedium
    {
        NONE,
        ANY,
        ARABINOSE
    }

    public enum DeviceAction
    {
        NONE,
        MOVE,
        PRODUCE,
        EMIT
    }
}
