using System;
using System.Collections.Generic;
using System.Text;
using Bomber.Domain.Bombs;
using Bomber.Application.Services;
using Bomber.Domain.Players;

namespace Bomber.Application.Commands
{
    public class PlaceBombCommand : IBombCommand
    {
        private readonly GameFacade gameStateService;
        private readonly Guid playerId;
        private Guid? bombId;

        public PlaceBombCommand(GameFacade service, Guid playerId)
        {
            this.gameStateService = service;
            this.playerId = playerId;
        }
        
        public bool Execute()
        {
            bool success = gameStateService.PlaceBomb(playerId, out Bomb? bomb);

            if (success && bomb is not null)
                this.bombId = bomb.Id;
            
            return success;
        }

        public bool Undo()
        {
            if (this.bombId is null)
                return false;
            
            if(this.bombId is Guid bombId)
                return gameStateService.RemoveBomb(bombId);

            return false;
        }
        public Guid GetPlayerId()
        {
            return playerId;
        }
    }
}
