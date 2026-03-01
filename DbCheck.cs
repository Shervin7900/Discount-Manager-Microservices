using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

class DbCheck
{
    static async Task Main()
    {
        Console.WriteLine("--- Database Connection Check ---");

        // SQL Server
        try {
            string sqlConn = "Server=localhost;Database=master;User Id=sa;Password=Password123!;TrustServerCertificate=True;Connect Timeout=5";
            using var connection = new SqlConnection(sqlConn);
            await connection.OpenAsync();
            Console.WriteLine("[PASS] SQL Server: Connected successfully.");
        } catch (Exception ex) {
            Console.WriteLine($"[FAIL] SQL Server: {ex.Message}");
        }

        // Redis
        try {
            string redisConn = "localhost:6379,connectTimeout=2000";
            var redis = ConnectionMultiplexer.Connect(redisConn);
            if (redis.IsConnected) {
                Console.WriteLine("[PASS] Redis: Connected successfully.");
            } else {
                Console.WriteLine("[FAIL] Redis: Could not connect.");
            }
        } catch (Exception ex) {
            Console.WriteLine($"[FAIL] Redis: {ex.Message}");
        }

        // MongoDB
        try {
            string mongoConn = "mongodb://localhost:27017";
            var client = new MongoClient(new MongoClientSettings { 
                Server = new MongoServerAddress("localhost", 27017),
                ConnectTimeout = TimeSpan.FromSeconds(2)
            });
            await client.ListDatabaseNamesAsync();
            Console.WriteLine("[PASS] MongoDB: Connected successfully.");
        } catch (Exception ex) {
            Console.WriteLine($"[FAIL] MongoDB: {ex.Message}");
        }
    }
}
