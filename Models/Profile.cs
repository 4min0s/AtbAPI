using System;
using System.Collections.Generic;

namespace TodoApi.Models;

public partial class Profile
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string MotDePasse { get; set; } = null!;

    public int? ClientId { get; set; }

    public string? Otp { get; set; }

    public DateTime? OtpExpiry { get; set; }

    public virtual Client? Client { get; set; }
}
