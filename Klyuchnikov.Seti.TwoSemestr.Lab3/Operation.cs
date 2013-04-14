using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Klyuchnikov.Seti.TwoSemestr.Lab3
{
    [Flags]
    enum Operation
    {
        Login,
        Password,
        DislayProcessed,
        RequestInfoSite,
        DeleteInfoSite,
        StartInfoSite,
        StopInfoSite,
        WaitOperation,
        WaitOfContinued
    }
}
