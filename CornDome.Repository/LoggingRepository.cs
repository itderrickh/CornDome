﻿using CornDome.Models.Logging;
using Dapper;

namespace CornDome.Repository
{
    public interface ILoggingRepository
    {
        Task<int> LogRouteChange(string fromRoute, string toRoute);
        Task<IEnumerable<RouteLog>> GetAllRouteLogs();
    }

    public class LoggingRepository(IDbConnectionFactory dbConnectionFactory) : ILoggingRepository
    {
        private const string INSERT_QUERY = @"
            INSERT INTO RouteLogging (FromRoute, ToRoute, HitCount)
            VALUES (@FromRoute, @ToRoute, 1)
            ON CONFLICT(FromRoute, ToRoute) 
            DO UPDATE SET HitCount = excluded.HitCount + RouteLogging.HitCount;
            ";

        private const string GETALL_QUERY = @"
            SELECT FromRoute, ToRoute, HitCount FROM RouteLogging;
        ";

        public async Task<int> LogRouteChange(string fromRoute, string toRoute)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var result = await con.ExecuteAsync(INSERT_QUERY, new { FromRoute = fromRoute, ToRoute = toRoute });

            return result;
        }

        public async Task<IEnumerable<RouteLog>> GetAllRouteLogs()
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var results = await con.QueryAsync<RouteLog>(GETALL_QUERY);

            return results;
        }
    }
}
