using System;
using System.Collections.Generic;
using Bomber.Client.Rendering.Animations;
using Bomber.Client.Models;

namespace Bomber.Domain.Players
{
    internal class PlayerDeadState : IAnimationState
    {
        public PlayerDeadState()
        {

        }

        public void Update(float deltaTime)
        {
        }

        public string GetSheetName()
        {
            return "dead";
        }

        public int GetSpriteRow(PlayerStateResponse player)
        {
            return 0;
        }

        public int GetSpriteColumn(PlayerStateResponse player)
        {
            return 3;
        }
    }
}
