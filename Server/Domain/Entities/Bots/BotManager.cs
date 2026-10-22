using Bomber.Domain.Bombs;
using Bomber.Domain.Builders;
using Bomber.Domain.Common;
using Bomber.Domain.Maps;
using Bomber.Domain.Players;

namespace Bomber.Domain.Entities.Bots
{
    public sealed class BotManager
    {
        private static BotManager? instance;
        private static readonly object _lock = new();
        const float STRATEGY_UPDATE_FREQUENCY = 3f;

        private readonly BotDirector director;

        Dictionary<Guid, Bot> bots = new();
        float strategy_updates = 0;

        Dictionary<Guid, Player> players;
        Astar astar;

        public Map GameMap { private set; get; }
        Random rng;

        public delegate List<IBomb> GetBombListHandler();
        public GetBombListHandler GetBombList;
        public delegate void OnBombRequestHandler(Guid id);
        public OnBombRequestHandler OnBombRequest;


        private BotManager( Map map, Dictionary<Guid, Player> players)
        {
            OnBombRequest = _ => { };
            GetBombList = () => new();
            rng = new();
            GameMap = map;
            this.players = players;
            astar = new(map);
            director = new BotDirector();
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

        public void RemoveBot(Guid id)
        {
            bots.Remove(id);
        }
        public Bot CreateHunter(Position position)
        {
            Bot bot = director.ConstructHunter(new HunterBotBuilder(), position);
            bots.Add(bot.Id, bot);
            return bot;
        }

        public Bot CreateDefender(Position position)
        {
            Bot bot = director.ConstructDefender(new DefenderBotBuilder(), position);
            bots.Add(bot.Id, bot);
            return bot;
        }

        public void Update(float deltaTime)
        {
            strategy_updates += deltaTime;

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

        public void UpdateBotStrategy(Bot bot)
        {
            List<(BotStrategy, float)> strategies = new(GetAllStrategies(bot).Select(s => (s, s.GetWeight())));

            float totalWeight = strategies.Select(s => s.Item2).Sum();

            if(totalWeight == 0)
            {
                bot.SetStrategy(strategies[0].Item1);
                return;
            }

            float random = rng.NextSingle() * totalWeight;
            float current = 0;

            foreach(var s in strategies)
            {
                current += s.Item2;
                if(current >= random)
                {
                    bot.SetStrategy(s.Item1);
                    return;
                }
            }
        }

        List<BotStrategy> GetAllStrategies(Bot bot)
        {
            return new()
            {
                new HuntStrategy(bot),
                new PowerUpStrategy(bot),
                new BombAvoidStrategy(bot),
                new ExploreStrategy(bot)
            };
        }

        public Player GetClosestPlayer(Player player)
        {
            Player closest = new(Guid.Empty, "", new(-1, -1), 0, 0, 0, 0);
            float cDistance = float.MaxValue;

            foreach (var p in players)
            {
                if (p.Value == player || p.Value.State == Domain.Enums.PlayerState.Dead)
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
