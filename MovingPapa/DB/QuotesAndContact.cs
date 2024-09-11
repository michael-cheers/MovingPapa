using System;
using System.Collections.Generic;

namespace MovingPapa.DB;

public partial class QuotesAndContact
{
    public int Id { get; set; }

    public DateTime TimeCreated { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public bool IsMuscleOnly { get; set; }

    public string Addresses { get; set; } = null!;

    public string Uuid { get; set; } = null!;

    public string? MoveInfo { get; set; }

    public string? Packages { get; set; }

    public DateTime? TimeUpdated { get; set; }

    public bool IsCallNow { get; set; }

    public DateTime MoveDate { get; set; }

    public string MoveTime { get; set; } = null!;

    public virtual ICollection<Move> Moves { get; set; } = new List<Move>();
}
