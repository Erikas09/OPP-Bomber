using Bomber.Client.Models;
using Bomber.Domain.Players;
using System.Diagnostics;

namespace Bomber.Client.Rendering.Animations;

public class PlayerAnimator
{
    PlayerStateResponse _lastPlayerData;
    IAnimationState _currentState;
    

    public PlayerAnimator(PlayerStateResponse player)
    {
        _lastPlayerData = player;
        _currentState = new PlayerIdleState();
    }

    public void UpdatePlayerData(PlayerStateResponse player)
    {
        _lastPlayerData = player;
    }

    public void Update(float deltaTime)
    {
        if (_lastPlayerData.IsWinner)
        {
            ChangeState(new PlayerVictoryState());
        }
        else if (!_lastPlayerData.IsAlive)
        {
            if(_currentState is not PlayerDeadState)
                ChangeState(new PlayerDyingState(this));
        }
        else if (_lastPlayerData.IsMoving)
        {
            ChangeState(new PlayerMoveState());
        }
        else
        {
            ChangeState(new PlayerIdleState());
        }

        _currentState.Update(deltaTime);
    }

    public void ChangeState(IAnimationState state)
    {
        if (_currentState.GetType() == state.GetType())
            return;

        _currentState = state;
    }

    public string GetSheetName()
    {
        return _currentState.GetSheetName();
    }

    public int GetSpriteSheetRow()
    {
        return _currentState.GetSpriteRow(_lastPlayerData);
    }

    public int GetSpriteSheetColumn()
    {
        return _currentState.GetSpriteColumn(_lastPlayerData);
    }
}
