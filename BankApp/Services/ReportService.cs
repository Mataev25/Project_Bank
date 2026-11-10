using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Npgsql;


namespace BankApp.Services {
    public class ReportService {
        private string conctString;

        public ReportService(string conctString) {
            this.conctString = conctString;
        }

        public List<UserCardBalance> GetUserCardBalanceReport() {
            var result = new List<UserCardBalance>();
            NpgsqlConnection connect = null;
            NpgsqlCommand command = null;
            NpgsqlDataReader reader = null;

            try {
                connect = new NpgsqlConnection(conctString);
                connect.Open();

                string query = @"
                    SELECT u.""FullName"", c.""Number"", a.""Balance""
                    FROM ""Users"" u
                    JOIN ""Cards"" c ON u.""Id"" = c.""UserId""
                    JOIN ""Accounts"" a ON u.""Id"" = a.""UserId""
                ";

                command = new NpgsqlCommand(query, connect);
                reader = command.ExecuteReader();

                while(reader.Read()) {
                    result.Add(new UserCardBalance{
                        FullName = reader["FullName"].ToString(),
                        CardNumber = reader["Number"].ToString(),
                        Balance = Convert.ToDecimal(reader["Balance"])

                    });
                } 
            } finally {
                if (reader != null) {
                    reader.Close();
                    reader.Dispose();
                }
                if (command != null) {
                    command.Dispose();
                }
                if (connect != null) {
                    connect.Close();
                    connect.Dispose();
                }
            }
            return result;
        }

        public void SaveToTxt(List<UserCardBalance> data, string filePath) {
            StreamWriter writer = null;
            try {
                writer = new StreamWriter(filePath);
                writer.WriteLine("===== Отчет: пользователи, карты, балансы ======");
                writer.WriteLine($"{"Имя",-30} {"Карта",-20} {"Баланс",12:F2}");
                writer.WriteLine(new string('-', 70));

                foreach (var row in data)
                    writer.WriteLine(
                        $"{row.FullName,-30} " +
                        $"{row.CardNumber,-20} " +
                        $"{row.Balance,12:F2}");
            } finally {
                if (writer != null) {
                    writer.Close();
                    writer.Dispose();
                }
            }
        }

        public void SaveToCsv(List<UserCardBalance> data, string filePath) {
            StreamWriter writer = null;
            try {
                writer = new StreamWriter(filePath);
                writer.WriteLine("FullName, CardNumber, Balance");
                foreach (var row in data)
                    writer.WriteLine(
                        $"{row.FullName} ",
                        $"{row.CardNumber} ",
                        $"{row.Balance.ToString("F2")}");
            } finally {
                if (writer != null) {
                    writer.Close();
                    writer.Dispose();
                }
            }
        }

        public void SaveToJson(List<UserCardBalance> data, string filePath) {
            FileStream file = null;
            try {
                var options = new JsonSerializerOptions {WriteIndented = true};
                string json = JsonSerializer.Serialize(data, options);
                byte[] bytes = Encoding.UTF8.GetBytes(json);

                file = new FileStream(
                        filePath,
                        FileMode.Create,
                        FileAccess.Write);
                        
                file.Write(bytes, 0, bytes.Length);
            } finally {
                if (file != null) {
                    file.Close();
                    file.Dispose();
                }
            }
        }
    }

    public class UserCardBalance {
        public string FullName {get; set;}
        public string CardNumber {get; set;}
        public decimal Balance {get; set;}
    }
}