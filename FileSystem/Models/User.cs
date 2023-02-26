using System;
using System.Collections.Generic;

namespace FileSystem.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<Document> Documents { get; } = new List<Document>();
}
