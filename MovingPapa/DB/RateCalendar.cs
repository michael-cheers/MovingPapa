using System;
using System.Collections.Generic;

namespace MovingPapa.DB;

public partial class RateCalendar
{
    public DateTime Date { get; set; }

    public int RatePerMoverInCents { get; set; }
}
