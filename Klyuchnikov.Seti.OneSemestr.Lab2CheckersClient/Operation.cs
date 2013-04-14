using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Klyuchnikov.Seti.OneSemestr.Lab2CheckersClient
{
    public enum Operation : byte
    {
        Assert = 0,
        UserName = 1,
        Close = 200,
        ListUsers = 2,
        SubmitGame = 3,
        TakeGame = 4,
        Stroke = 5,
        AbortOpponentConnection = 6,
        OfferDraw = 7,
        GiveUp = 8,
        AgreeToDraw = 9
    }
}
