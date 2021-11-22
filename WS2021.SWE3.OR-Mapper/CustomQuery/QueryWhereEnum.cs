using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS2021.SWE3.OR_Mapper.CustomQuery
{
    public enum QueryWhereActions
    {
        NOP = 0,
        NOT = 1,
        GRP = 2,
        ENDGRP = 3,
        EQUALS = 4,
        LIKE = 5,
        IN = 6,
        GT = 7,
        LT = 8
    }

    public enum QueryWhereConnections
    {
        NONE = 0,
        AND = 1,
        OR = 2,
    }
}
