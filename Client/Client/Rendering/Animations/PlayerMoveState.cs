
using System;
using System.Collections.Generic;
using System.Text;
using Bomber.Client.Models;
using Bomber.Client.Rendering.Animations;

namespace Bomber.Domain.Players
{
    internal class PlayerMoveState : IAnimationState
    {
        static float ANIMATION_TIME = 1f;
        static int FRAME_COUNT = 4;

        float _currentTime;

        public PlayerMoveState()
        {

        }

        public void Update(float deltaTime)
        {
            _currentTime += deltaTime;

            if (_currentTime > ANIMATION_TIME)
                _currentTime = 0;
        }

        public string GetSheetName()
        {
            return "move";
        }

        public int GetSpriteRow(PlayerStateResponse player)
        {
            return (int)player.MoveDirection;
        }

        public int GetSpriteColumn(PlayerStateResponse player)
        {
            return (int)(_currentTime / (ANIMATION_TIME / FRAME_COUNT));
        }
    }
}
