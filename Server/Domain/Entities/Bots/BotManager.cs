using Bomber.Domain.Common;
using Bomber.Domain.Maps;
using Bomber.Domain.Players;

namespace Bomber.Domain.Entities.Bots
{
    public sealed class BotManager
    {
        private static BotManager? instance;
        private static readonly object _lock = new();
        const int STRATEGY_UPDATE_FREQUENCY = 200;

        Dictionary<Guid, Bot> bots = new();
        float strategy_updates = 0;

        Dictionary<Guid, Player> players;
        Astar astar;

        private BotManager(Map map, Dictionary<Guid, Player> players)
        {
            this.players = players;
            astar = new(map);
        }

        public static BotManager Instance
        {
            get
            {
                lock (_lock)
                {
                    return instance
                        ?? throw new InvalidOperationException(
                            "BotManager is not initialized.");
                }
            }
        }

        public static void Initialize(
                Map map,
                Dictionary<Guid, Player> players)
        {
            lock (_lock)
            {
                if (instance != null)
                    throw new InvalidOperationException(
                        "BotManager is already initialized.");

                instance = new BotManager(map, players);
            }
        }

        public Bot CreateBot(Position position)
        {
            Bot bot = Create(position);
            bots.Add(bot.Id, bot);

            UpdateBotStrategy(bot);

            return bot;
        }

        public void RemoveBot(Guid id)
        {
            bots.Remove(id);
        }

        Bot Create(Position position)
        {
            return new(Guid.NewGuid(), "Bot", position, 3, 1, 2, 120f);
        }

        public void Update(float deltaTime)
        {
            strategy_updates++;

            if (strategy_updates >= STRATEGY_UPDATE_FREQUENCY)
            {
                UpdateAllStrategies();
                strategy_updates = 0;
            }

            foreach (var bot in bots)
                bot.Value.Update(deltaTime);

        }

        void UpdateAllStrategies()
        {
            foreach (var bot in bots)
                UpdateBotStrategy(bot.Value);
        }

        void UpdateBotStrategy(Bot bot)
        {
            bot.SetStrategy(new HuntStrategy());
        }

        public Player GetClosestPlayer(Player player)
        {
            Player closest = new(Guid.Empty, "", new(-1, -1), 0, 0, 0, 0);
            float cDistance = float.MaxValue;

            foreach(var p in players)
            {
                if (p.Value == player || p.Value is Bot)
                    continue;

                float distance = (p.Value.Position.X - player.Position.X) * (p.Value.Position.X - player.Position.X) +
                    (p.Value.Position.Y - player.Position.Y) * (p.Value.Position.Y - player.Position.Y);

                if (distance < cDistance)
                {
                    cDistance = distance;
                    closest = p.Value;
                }
            }

            return closest;
        }

        public Stack<GridPosition> FindPath(GridPosition start, GridPosition target)
        {
            return astar.FindPath(start, target);
        }
    }
}
