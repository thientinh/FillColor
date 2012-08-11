using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ToMau
{
    interface IStrategy
    {
        int Fill(Context context);
    }
}
