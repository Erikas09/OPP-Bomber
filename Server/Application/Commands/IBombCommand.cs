using Bomber.Domain.Players;
using System;
using System.Collections.Generic;
using System.Text;
using Bomber.Domain.Bombs;

namespace Bomber.Application.Commands
{
    internal interface IBombCommand
    {
        public abstract bool Execute(out Bomb? bomb);
        public abstract bool Undo();
    }
}
