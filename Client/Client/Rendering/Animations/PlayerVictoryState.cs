using System;
using System.Collections.Generic;
using System.Text;
using Bomber.Client.Models;

namespace Bomber.Client.Rendering.Animations
{
    internal class PlayerVictoryState : IAnimationState
    {
        static float ANIMATION_TIME = 1f;
        static int FRAME_COUNT = 2;

        float _currentTime;

        public PlayerVictoryState()
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
            return "victory";
        }

        public int GetSpriteRow(PlayerStateResponse player)
        {
            return 0;
        }

        public int GetSpriteColumn(PlayerStateResponse player)
        {
            return (int)(_currentTime / (ANIMATION_TIME / FRAME_COUNT));
        }
    }
}
