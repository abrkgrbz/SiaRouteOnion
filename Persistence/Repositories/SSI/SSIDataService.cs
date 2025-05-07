using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Application.Interfaces.SSIServices;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Persistence.Contexts;

namespace Persistence.Repositories.SSI
{
    public class SSIDataService : ISSIDataService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IQuestionRepositoryAsync _questionRepository;
        private readonly IResponseRepositoryAsync _responseRepository;
        private readonly IMemoryCache _memoryCache;
        public SSIDataService(IServiceProvider serviceProvider, IResponseRepositoryAsync responseRepository,
            IQuestionRepositoryAsync questionRepository, IMemoryCache memoryCache)
        {
            _serviceProvider = serviceProvider;
            _responseRepository = responseRepository;
            _questionRepository = questionRepository;
            _memoryCache = memoryCache;
        }

        private async Task<Dictionary<int, string>> GetMapList(string projectName)
        {
            string tableName = $"{projectName}_map";
            using (var scope = _serviceProvider.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<SiaRouteSSIDbContext>();
                var mapList = await _dbContext.GetDynamicMapList(tableName);
                Dictionary<int, string> result = new Dictionary<int, string>();
                foreach (var map in mapList)
                {

                    string[] fieldsArray = map.fields.Split(',');
                    string fieldsWithBracketsAndCommas =
                        string.Join(", ", fieldsArray.Select(f => $"data{map.table}.[{f.Trim()}]"));

                    result.Add(map.table, fieldsWithBracketsAndCommas);
                }

                return result;
            }
        } 
        private async Task<List<Dictionary<string, object>>> GetDynamicData(string projectName)
        {
            var mapList = await GetMapList(projectName);
            string joinQuery = "";
            bool isFirstTable = true;
            string fields = "";
            int index = 0;
            foreach (var map in mapList)
            {
                string tableName = $"{projectName}_data{map.Key} data{map.Key}";
                string joinCondition = $"data{map.Key}.sys_RespNum = data1.sys_RespNum";
                fields += map.Value;
                if (index < mapList.Count - 1)
                {
                    fields += ",";
                }

                if (isFirstTable)
                {
                    joinQuery += tableName;
                    isFirstTable = false;
                }
                else
                {
                    joinQuery += $" FULL JOIN {tableName} ON {joinCondition}";

                }

                index++;

            }

            string query = $"SELECT {fields} FROM {joinQuery}";

            var tableData = await ExecuteQuery(query);
            return tableData;

        } 
        private async Task<List<Dictionary<string, object>>> ExecuteQuery(string query)
        {
            var result = new List<Dictionary<string, object>>();
            using (var scope = _serviceProvider.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<SiaRouteSSIDbContext>();
                var connection = _dbContext.Database.GetDbConnection();
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }

                            result.Add(row);
                        }
                    }
                }

                await connection.CloseAsync();
            }

            return result;
        }
        private async Task<List<Dictionary<string, int>>> ExecuteQueryReturnInt(string query)
        {
            var result = new List<Dictionary<string, int>>();
            using (var scope = _serviceProvider.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<SiaRouteSSIDbContext>();
                var connection = _dbContext.Database.GetDbConnection();
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, int>();
                            var statusDescription = reader.GetString(0); // İlk sütun 
                            var totalCount = reader.GetInt32(1); // İkinci sütun
                            row[statusDescription] = totalCount;
                            result.Add(row);
                        }
                    }
                }

                await connection.CloseAsync();
            }

            return result;
        }
        public async Task<List<Dictionary<string, object>>> ConvertDataToDataTable(int projectId, string projectName)
        {
            var responses = await _responseRepository.GetWhereList(x => x.ProjectId == projectId);
            var responseDictionary = responses
                .GroupBy(x => x.ParentQuestionName)
                .ToDictionary(g => g.Key, g => g.Select(r => (r.ResponseValue, r.ResponseText)).ToList());

            var questions = await _questionRepository.GetWhereList(x => x.ProjectId == projectId);
            var questionMultiple = questions.Where(x => x.IsMultiple == true).ToList();
            var dataList = await GetDynamicData(projectName);


            foreach (var data in dataList)
            {
                foreach (var key in responseDictionary.Keys)
                {
                    if (data.ContainsKey(key))
                    {
                        var responseValues = responseDictionary[key];
                        foreach (var (responseValue, responseText) in responseValues)
                        {
                            if (data[key].ToString() == responseValue.ToString())
                            {
                                data[key] = responseText;
                            }
                        }
                    }

                     
                }
            }

            foreach (var data in dataList)
            {
                
                foreach (var question in questions)
                { 
                    if (data.ContainsKey(question.QuestionName))
                    {
                        var newKey = $"{question.QuestionName} - {question.QuestionText}";
                        var value = data[question.QuestionName];

                        data[newKey] = value;
                        data.Remove(question.QuestionName);
                    }
                   
                     
                }

                foreach (var question in questionMultiple)
                {
                    
                    foreach (var response in responses)
                    {
                        if (question.QuestionName==response.ParentQuestionName)
                        {
                            var newKey = $"{response.QuestionName} - {question.QuestionText}-{response.ResponseText}";
                            var value = data[response.QuestionName];

                            data[newKey] = value;
                            data.Remove(response.QuestionName);
                            
                        }
                         

                    }
                }
            }
            return dataList;
        }

        public DataTable ConvertListToDataTable(List<Dictionary<string, object>> list)
        {
            DataTable table = new DataTable();
            if (list == null || !list.Any())
            {
                return table;
            }

            foreach (var key in list.First().Keys)
            {
                table.Columns.Add(key);
            }

            foreach (var dict in list)
            {
                var row = table.NewRow();
                foreach (var key in dict.Keys)
                {
                    row[key] = dict[key] ?? DBNull.Value;

                }

                table.Rows.Add(row);
            }

            return table;
        }

        public async Task<List<Dictionary<string, int>>> ProjectStatusDetails(string projectName)
        {
            string cacheKey = $"ProjectStatusDetails_{projectName}";
            if (_memoryCache.TryGetValue(cacheKey, out List<Dictionary<string, int>> cachedData))
            {
                return cachedData;
            }
            string tableName = $"{projectName}_data1";
            string query =
                $"SELECT \r\n  'Total' AS StatusDescription, \r\n    COUNT(*) AS TotalCount \r\nFROM \r\n    {tableName} \r\nWHERE \r\n    sys_RespStatus IN (2, 4, 5)\r\n\r\nUNION ALL\r\n\r\nSELECT \r\n    CASE \r\n  WHEN sys_RespStatus = 2 THEN 'Incomplete' \r\n        WHEN sys_RespStatus = 4 THEN 'Disqualified' \r\n        WHEN sys_RespStatus = 5 THEN 'Completed' \r\n    ELSE 'Other'  \r\n    END AS StatusDescription, \r\n    COUNT(*) AS TotalCount\r\n FROM \r\n  {tableName} \r\n WHERE \r\n  sys_RespStatus IN (2, 4, 5)\r\nGROUP BY \r\n    sys_RespStatus \r\n ORDER BY \r\n    StatusDescription;\r\n";
            var data = await ExecuteQueryReturnInt(query);
            var cacheEntyOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1),
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };
            _memoryCache.Set(cacheKey, data,cacheEntyOptions);
            return data;
        }

    }
}

