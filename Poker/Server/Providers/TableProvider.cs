using AutoMapper;
using Poker.Shared.Models.DomainModels;
using Poker.Shared.Models.PokerModels;
using Poker.Shared.Models.ViewModels;
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
        public TableProvider(Mapper mapper)
        {
            _mapper = mapper;
            Tables = CreateMockTables();
        }

        private List<Table> CreateMockTables()
        {
            var tables = new List<Table>();
            for (int i = 0; i < 5; i++)
            {
                tables.Add(new Table(i + 1, $"Table {i + 1}"));
            }
            return tables;
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
                table?.PokerUsers.Add(pokerUser);
                return true;
            }
            return false;
        }

        public bool LeaveTable(int tableId, PokerUser pokerUser)
        {
            if (Tables.Any(a => a.PokerUsers.Any(p => p.Username == pokerUser.Username)))
            {
                var table = Tables.FirstOrDefault(t => t.Id == tableId);
                var markedUser = table.PokerUsers.FirstOrDefault(p => p.Username == pokerUser.Username);
                var result = table?.PokerUsers.Remove(markedUser);
                return result.HasValue ? result.Value : false;
            }
            return false;
        }

        public void IncrementTables() {
            var tableCount = Tables.Count;
            var newTable = new Table(tableCount + 1, $"Table {tableCount + 1}");
            Tables.Add(newTable);
        }
    }
}
