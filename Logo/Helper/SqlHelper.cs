using Microsoft.Data.SqlClient;
using System.Data;

namespace GeaDesign001.Core.Helper
{
    public static class SqlHelper
    {
        // ------------------------------
        // ASYNC METHODS
        // ------------------------------
        public static async Task<int> ExecuteNonQueryAsync(string connectionString, CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(commandText, connection) { CommandType = commandType };
            if (parameters?.Length > 0)
                command.Parameters.AddRange(parameters);

            return await command.ExecuteNonQueryAsync();
        }
        public static async Task<DataTable> ExecuteDataTableAsync(string connectionString, CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(commandText, connection) { CommandType = commandType };
            if (parameters?.Length > 0)
                command.Parameters.AddRange(parameters);

            using var reader = await command.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }
        public static async Task<object?> ExecuteScalarAsync(string connectionString, CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(commandText, connection) { CommandType = commandType };
            if (parameters?.Length > 0)
                command.Parameters.AddRange(parameters);

            return await command.ExecuteScalarAsync();
        }
        // ------------------------------
        // SYNC METHODS
        // ------------------------------
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            using var command = new SqlCommand(commandText, connection) { CommandType = commandType };
            if (parameters?.Length > 0)
                command.Parameters.AddRange(parameters);
            command.CommandTimeout = 300; // wait up to 5 minutes
            return command.ExecuteNonQuery();
        }
        public static DataTable ExecuteDataTable(string connectionString, CommandType commandType, string commandText,  params SqlParameter[] parameters)
        {
            int timeout = 600;
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand(commandText, connection)
            {
                CommandType = commandType,
                CommandTimeout = timeout
            };

            if (parameters?.Length > 0)
                command.Parameters.AddRange(parameters);

            connection.Open();

            using var reader = command.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);

            return dt;
        }
        public static object? ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            using var command = new SqlCommand(commandText, connection) { CommandType = commandType };
            if (parameters?.Length > 0)
                command.Parameters.AddRange(parameters);

            return command.ExecuteScalar();
        }
    }
}

