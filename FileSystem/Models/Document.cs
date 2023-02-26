using System;
using System.Collections.Generic;

namespace FileSystem.Models;

public partial class Document
{
    public int Id { get; set; }

    public int? Uid { get; set; }

    public string? FileName { get; set; }

    public string? ContentType { get; set; }

    public string? FilePath { get; set; }

    public long? Length { get; set; }

    public virtual User? UidNavigation { get; set; }
}
