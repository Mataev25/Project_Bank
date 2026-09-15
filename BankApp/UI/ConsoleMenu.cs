using System;
using BankApp.Models;
using BankApp.Services;
using System.Collections.Generic;
using BankApp.Data;

namespace BankApp.UI {
    public class ConsoleMenu : IMenu {
        private AuthService authService;
        private User currentUser;
        private AccountService accountService;
        private AppDbContext context;

        public ConsoleMenu() {
            context = new AppDbContext();
            authService = new AuthService(context);
            currentUser = null;
            accountService = new AccountService(context);

        }

        public void Run() {
            Console.Clear();
            Console.WriteLine("Добро пожаловать в Е-банк\n");
            ShowAuthMenu();
        }

        public void ShowAuthMenu() {
            Console.WriteLine("#1. Вход по номеру карты");
            Console.WriteLine("#2. Выход");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();
            switch (choice) {
                case "1" : ShowLogin(); break;
                case "2" : ShowExit(); break;
                default: 
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    ShowAuthMenu();
                    break;
            }
        }

        private void ShowLogin() {
            Console.Write("Ввведите номер карты: ");
            string cardNumber = Console.ReadLine();

            if (!authService.CardExists(cardNumber)) {
                Console.WriteLine("Карта не найдена.");
                ShowAuthMenu();
                return;
            }

            if (authService.IsCardBlocked(cardNumber)) {
                Console.WriteLine("Карта заблокирована");
                ShowAuthMenu();
                return;
            }

            User user = authService.GetUserByCard(cardNumber);
            if (user == null) {
                Console.WriteLine("Пользователь не найден");
                ShowAuthMenu();
                return;
            }

            if (user.IsFirstLogin) {
                Console.Write("Это ваш первый вход. Установите PIN-код: ");
                string pin = Console.ReadLine();
                if (authService.ActivateCard(cardNumber, pin)) {
                    Console.WriteLine("Карта активирована.");
                    currentUser = user;
                    ShowAccountMenu();
                }
                else {
                    Console.WriteLine("Ошибка активации.");
                    ShowAuthMenu();
                }
            }
            else {
                Console.Write("Введите PIN-код: ");
                string pin = Console.ReadLine();
                if (authService.ValidatePin(cardNumber, pin)) {
                    Console.WriteLine($"Добро поаловать, {user.FullName}!");
                    currentUser = user;
                    authService.ResetFailedAttempts(cardNumber);
                    ShowAccountMenu();
                }
                else {
                    authService.IncrementFailedAttempts(cardNumber);
                    Console.WriteLine("Неверный PIN-код.");
                    if (authService.IsCardBlocked(cardNumber))
                        Console.WriteLine("Карта заблокирована после 3 неудачных попыток.");
                    
                    ShowAuthMenu();
                }
            }
        }

        public void ShowAccountMenu() {
            Console.WriteLine("\n********* Главное меню **********");
            Console.WriteLine("#1 Просмотр баланса");
            Console.WriteLine("#2 Пополнение счета");
            Console.WriteLine("#3 Снятие наличных");
            Console.WriteLine("#4 История операций");
            Console.WriteLine("#5 Выход из аккаунта");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();
            switch (choice) {
                case "1" : ShowBalance(); break;
                case "2" : ShowDeposit(); break;
                case "3" : ShowWithdraw(); break;
                case "4" : ShowTransactionHistory(); break;
                case "5" : 
                    currentUser = null;
                    Console.WriteLine("Вы вышли из аккаунта.");
                    ShowAuthMenu();
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    ShowAccountMenu();
                    break;
            }
        }

        private void ShowBalance() {
            if (currentUser == null) {
                Console.WriteLine("Пользователь не авторизован");
                ShowAuthMenu();
                return;
            }

            decimal balance = accountService.GetBalance(currentUser.Id);
            Console.WriteLine($"Баланс: {balance} р.");
            Console.Write("Нажмите Enter для продолжения...");
            Console.ReadLine();
            ShowAccountMenu();
        }

        private void ShowDeposit() {
            if (currentUser == null) {
                Console.WriteLine("Пользователь не авторизован");
                ShowAuthMenu();
                return;
            }

            Console.Write("Введите сумму для пополнения: ");
            string input = Console.ReadLine();
            if (!decimal.TryParse(input, out decimal amount)) {
                Console.WriteLine("Неверный формат суммы.");
                Console.WriteLine("Нажминте Enter для продолжения...");
                Console.ReadLine();
                ShowAccountMenu();
                return;
            }

            accountService.Deposit(currentUser.Id, amount);
            Console.WriteLine("Нажминте Enter для продолжения...");
            Console.ReadLine();
            ShowAccountMenu();
        }

        private void ShowWithdraw() {
             if (currentUser == null) {
                Console.WriteLine("Пользователь не авторизован");
                ShowAuthMenu();
                return;
            }
            Console.WriteLine("Введите сумму для снятия наличных: ");
            string input = Console.ReadLine();
            if (!decimal.TryParse(input, out decimal amount)) {
                Console.WriteLine("Неверный формат суммы.");
                Console.WriteLine("Нажминте Enter для продолжения...");
                Console.ReadLine();
                ShowAccountMenu();
                return;
            }

            accountService.Withdraw(currentUser.Id, amount);
            Console.WriteLine("Нажминте Enter для продолжения...");
            Console.ReadLine();
            ShowAccountMenu();
        }

        private void ShowTransactionHistory() {
            if (currentUser == null) {
                Console.WriteLine("Пользователь не авторизован");
                ShowAuthMenu();
                return;
            }
            Console.WriteLine("\n***** История операций *****");

            List<Transaction> userTransactions = accountService.GetTransactionsByUserId(currentUser.Id, 10);

            if (userTransactions.Count == 0)
                Console.WriteLine("Операций пока нет");
            else {
                int count = userTransactions.Count > 10 ? 10 : userTransactions.Count;
                for (int i = userTransactions.Count-1; i >= userTransactions.Count-count; i--) {
                    Transaction t = userTransactions[i];
                    Console.WriteLine($"{t.Date:dd.MM.yyyy HH:mm} | {t.Type} | {t.Amount} р. {t.Description}"); 
                }
            }
            Console.WriteLine("Нажминте Enter для продолжения...");
            Console.ReadLine();
            ShowAccountMenu();
        }

        public void ShowMainMenu() {

        }

        public void ShowExit() {
            Console.WriteLine("До свидания!");

        }
    }
}