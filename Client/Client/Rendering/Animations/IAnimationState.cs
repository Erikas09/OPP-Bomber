using Bomber.Client.Models;
using System;
using System.Collections.Generic;

namespace Bomber.Client.Rendering.Animations
{
    public interface IAnimationState
    {
        void Update(float deltaTime);
        string GetSheetName();
        int GetSpriteRow(PlayerStateResponse player);
        int GetSpriteColumn(PlayerStateResponse player);
    }
}
