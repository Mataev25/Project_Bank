using System;
using BankApp.Data;
using BankApp.Models;
using BankApp.Tests;
using BankApp.UI;
using BankApp;
using System.IO;
using BankApp.Services;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;


class Program {
    static void Main(string[] args) {
        if (args.Length > 0 && args[0] == "--report") {
            GenerateReports();
            return;
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        
        string connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        AppDbContext context = null;
        try {
            context = new AppDbContext(optionsBuilder.Options);
            DbInitializer.Seed(context);

            IMenu menu = new ConsoleMenu(context);
            menu.Run();
        } finally {
            if (context != null)
                context.Dispose();
        }


        //UserTests.Run();
        //AccountTests.Run();
        //AuthTests.Run();
    }

    static void GenerateReports() {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        string connectString = configuration.GetConnectionString("DefaultConnection");

        var reportService = new ReportService(connectString);
        Console.WriteLine("Генерация отчетов...");
        var data = reportService.GetUserCardBalanceReport();

        string baseName = $"report_{DateTime.Now:yyyyMMdd_HHmmss}";
        string root = Directory
            .GetParent(Environment.CurrentDirectory)
            .Parent
            .Parent
            .FullName;

        string folder = Path.Combine(Environment.CurrentDirectory, "Reports");
        Directory.CreateDirectory(folder);

        ShowReportInConsole(data);
        Console.Write("\nСохранить отчет в файлы? (y/n:) ");
        string ans = Console.ReadLine();

        if (ans == "y") {
            reportService.SaveToTxt(data, Path.Combine(folder, baseName + ".txt"));
            reportService.SaveToCsv(data, Path.Combine(folder, baseName + ".csv"));
            reportService.SaveToJson(data, Path.Combine(folder, baseName + ".json"));

            Console.WriteLine($" Отчеты сохранены в папку {folder}");
            Console.WriteLine($"     -{baseName}.txt");
            Console.WriteLine($"     -{baseName}.csv");
            Console.WriteLine($"     -{baseName}.json");
        }
    }

    static void ShowReportInConsole(List<UserCardBalance> data) {
        Console.WriteLine("===== Отчет: пользователи, карты, балансы ======");
        Console.WriteLine($"{"Имя",-30} {"Карта",-20} {"Баланс",12:F2}");
        Console.WriteLine(new string('-', 70));

        foreach (var row in data)
            Console.WriteLine($"{row.FullName,-30} {row.CardNumber,-20} {row.Balance,12:F2}");
    }
}
