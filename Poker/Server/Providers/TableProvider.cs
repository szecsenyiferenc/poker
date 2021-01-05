using AutoMapper;
using Poker.Shared;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
using Poker.Shared.Models.ViewModels;
using Poker.Shared.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Server.Providers
{
    public class TableProvider
    {
        public List<Table> Tables { get; set; }
        private readonly Mapper _mapper;
        private readonly IHubEventEmitter _hubEventEmitter;
        private readonly IEventProxy _eventProxy;

        public TableProvider(Mapper mapper,
            IHubEventEmitter hubEventEmitter,
            IEventProxy eventProxy)
        {
            _mapper = mapper;
            _hubEventEmitter = hubEventEmitter;
            _eventProxy = eventProxy;
            Tables = CreateMockTables();
        }

        private List<Table> CreateMockTables()
        {
            var tables = new List<Table>();
            for (int i = 0; i < 5; i++)
            {
                tables.Add(new Table(_hubEventEmitter, _eventProxy, i + 1, $"Table {i + 1}"));
            }
            return tables;
        }

        public Table GetCurrentTable(int tableId)
        {
            return Tables.FirstOrDefault(t => t.Id == tableId);
        }

        public Table GetCurrentTable(PokerUser pokerUser)
        {
            return Tables.FirstOrDefault(t => t.PokerUsers.Any(t => t.Username == pokerUser.Username));
        }

        public List<TableViewModel> GetAllTableViews()
        {
            var list = new List<TableViewModel>();
            foreach (var item in Tables)
            {
                list.Add(_mapper.Map<TableViewModel>(item));
            }
            return list;
        }

        public bool JoinToTable(int tableId, PokerUser pokerUser)
        {
            if(!Tables.Any(a => a.PokerUsers.Any(p => p.Username == pokerUser.Username)))
            {
                var table = Tables.FirstOrDefault(t => t.Id == tableId);
                table?.AddPlayer(pokerUser);
                return true;
            }
            return false;
        }

        public int LeaveTable(PokerUser pokerUser)
        {
            if (Tables.Any(a => a.PokerUsers.Any(p => p.Username == pokerUser.Username)))
            {
                var table = Tables.FirstOrDefault(a => a.PokerUsers.Any(p => p.Username == pokerUser.Username));
                var markedUser = table.PokerUsers.FirstOrDefault(p => p.Username == pokerUser.Username);
                var result = table?.RemovePlayer(markedUser);
                return table.Id;
            }
            return -1;
        }

        public int LeaveTable(int tableId, PokerUser pokerUser)
        {
            if (Tables.Any(a => a.PokerUsers.Any(p => p.Username == pokerUser.Username)))
            {
                var table = Tables.FirstOrDefault(t => t.Id == tableId);
                var markedUser = table.PokerUsers.FirstOrDefault(p => p.Username == pokerUser.Username);
                var result = table?.RemovePlayer(markedUser);
                return tableId;
            }
            return -1;
        }

        public void IncrementTables() {
            var tableCount = Tables.Count;
            var newTable = new Table(_hubEventEmitter, _eventProxy, tableCount + 1, $"Table {tableCount + 1}");
            Tables.Add(newTable);
        }
    }
}
