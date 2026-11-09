using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
namespace DCe.Utils.Abstractions;

/// <summary>Relógio injetável. Existe para que o XML gerado seja determinístico em teste.</summary>
public interface IClock
{
    DateTime UtcNow { get; }
}

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
