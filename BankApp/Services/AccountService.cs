using System;
using System.Collections.Generic;
using BankApp.Models;
using BankApp.Data;

namespace BankApp.Services {
    public class AccountService {
        private AppDbContext context;

        public AccountService(AppDbContext context) {
            this.context = context;
        }

        public Account GetAccountByUserId(int userId) {
            foreach (Account account in context.Accounts) {
                if (account.UserId == userId)
                    return account;
            }
            return null;
        }

        public decimal GetBalance(int userId) {
            Account account = GetAccountByUserId(userId);
            if (account == null) return 0;
            return account.Balance;
        }

        public bool Deposit(int userId, decimal amount) {
            if (amount <= 0) {
                Console.WriteLine("Сумма должна быть больше 0");
                return false;
            }

            Account account = GetAccountByUserId(userId);
            if (account == null) {
                Console.WriteLine("Счет не найден");
                return false;
            }

            account.Balance += amount;

            Transaction trans = new Transaction(
                                userId,
                                "Deposit", 
                                amount,
                                $"Пополнение счета {account.AccountNumber}");

            trans.Date = DateTime.Now;
            context.Transactions.Add(trans);

            context.SaveChanges();

            Console.WriteLine(
                $"Счет пополнен на {amount} р. " +
                $"Текущий баланс: {account.Balance} р.");
            return true;
        }

        public bool Withdraw(int userId, decimal amount) {
            if (amount <= 0) {
                Console.WriteLine("Сумма должна быть больше 0");
                return false;
            }

            Account account = GetAccountByUserId(userId);
            if (account == null) {
                Console.WriteLine("Счет не найден");
                return false;
            }

            if (account.Balance < amount) {
                Console.WriteLine(
                    $"Недостаточно средств. " +
                    $"Доступно: {account.Balance} р.");
                return false;
            }

            account.Balance -= amount;

            Transaction trans = new Transaction(
                                userId,
                                "Withdraw",
                                amount,
                                $"Снятие со счета {account.AccountNumber}");
                                
            trans.Date = DateTime.Now;
            context.Transactions.Add(trans);

            context.SaveChanges();
    
            Console.WriteLine(
                $"Снято: {amount} р. " +
                $"Текущий баланс: {account.Balance} р.");
            return true;
        }

        public List<Transaction> GetTransactionsByUserId(int userId, int limit = 10) {
            List<Transaction> result = new List<Transaction>();
            foreach (Transaction trans in context.Transactions) {
                if (trans.UserId == userId)
                    result.Add(trans);
            }

            result.Sort((a,b) => b.Date.CompareTo(a.Date));

            if (result.Count > limit)
                result = result.GetRange(0,limit);
            return result;
        }
        
    }


}