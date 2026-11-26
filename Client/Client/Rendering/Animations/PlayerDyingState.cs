using Bomber.Client.Models;

namespace Bomber.Client.Rendering.Animations
{
    internal class PlayerDyingState : IAnimationState
    {
        static float ANIMATION_TIME = 2f;
        static int FRAME_COUNT = 4;

        PlayerAnimator _animator;
        float _currentTime = 0;

        public PlayerDyingState(PlayerAnimator animator)
        {
            _animator = animator;
        }

        public void Update(float deltaTime)
        {
            if (_currentTime < ANIMATION_TIME)
                _currentTime += deltaTime;
            else
                _animator.ChangeState(new PlayerDeadState());
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
            if (_currentTime >= ANIMATION_TIME)
                return FRAME_COUNT - 1;


            return (int)(_currentTime / (ANIMATION_TIME / FRAME_COUNT));
        }
    }
}
