using System;
using System.Collections.Generic;
using System.Text;

namespace Bomber.Client.Models;

public readonly record struct GridPosition(
    int X,
    int Y);