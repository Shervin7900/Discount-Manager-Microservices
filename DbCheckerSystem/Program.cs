using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("--- Database Connection Status Report ---");
        Console.WriteLine($"Check Time: {DateTime.Now}");
        Console.WriteLine();

        // SQL Server
        Console.Write("1. SQL Server (localhost:1433)... ");
        try {
            string sqlConn = "Server=localhost;Database=master;User Id=sa;Password=Password123!;TrustServerCertificate=True;Connect Timeout=5";
            using var connection = new SqlConnection(sqlConn);
            await connection.OpenAsync();
            Console.WriteLine("CONNECTED");
        } catch (Exception ex) {
            Console.WriteLine("FAILED");
            Console.WriteLine($"   Error: {ex.Message}");
        }

        // Redis
        Console.Write("2. Redis (localhost:6379)...      ");
        try {
            string redisConn = "localhost:6379,connectTimeout=2000,abortConnect=false";
            var redis = ConnectionMultiplexer.Connect(redisConn);
            if (redis.IsConnected) {
                Console.WriteLine("CONNECTED");
            } else {
                Console.WriteLine("FAILED (Timeout)");
            }
        } catch (Exception ex) {
            Console.WriteLine("FAILED");
            Console.WriteLine($"   Error: {ex.Message}");
        }

        // MongoDB
        Console.Write("3. MongoDB (localhost:27017)...    ");
        try {
            string mongoConn = "mongodb://localhost:27017";
            var client = new MongoClient(new MongoClientSettings { 
                Server = new MongoServerAddress("localhost", 27017),
                ConnectTimeout = TimeSpan.FromSeconds(2),
                ServerSelectionTimeout = TimeSpan.FromSeconds(2)
            });
            await client.ListDatabaseNamesAsync();
            Console.WriteLine("CONNECTED");
        } catch (Exception ex) {
            Console.WriteLine("FAILED");
            Console.WriteLine($"   Error: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("------------------------------------------");
    }
}
