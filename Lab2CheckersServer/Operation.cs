using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab2CheckersServer
{
    public enum Operation : byte
    {
        UserName = 1,
        Close = 200,
        ListUsers = 2,
        SubmitGame = 3,
        TakeGame = 4,
        Stroke = 5
    }
}
