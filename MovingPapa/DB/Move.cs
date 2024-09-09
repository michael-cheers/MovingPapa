using System;
using System.Collections.Generic;

namespace MovingPapa.DB;

public partial class Move
{
    public int Id { get; set; }

    public string Uuid { get; set; } = null!;

    public string MoveDetails { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int PriceInCents { get; set; }

    public DateTime MoveDate { get; set; }

    public string MoveTime { get; set; } = null!;

    public DateTime TimeCreated { get; set; }

    public int? QuoteId { get; set; }

    public virtual QuotesAndContact? Quote { get; set; }
}
