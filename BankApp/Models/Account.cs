using System;

namespace BankApp.Models {
    public class Account {
        private int id;
        private int userId;
        private string accountNumber;
        private decimal balance;
        private DateTime createdAt;

        public Account (int userId, string accountNumber) {
            this.id = 0;
            this.userId = userId;
            this.accountNumber = accountNumber;
            this.balance = 0;
            this.createdAt = DateTime.UtcNow;
        }

        public int Id {
            get {return id;}
            set {id = value;}
        }

        public int UserId {
            get {return userId;}
            set {userId = value;}
        }

        public string AccountNumber {
            get {return accountNumber;}
            set {accountNumber = value;}
        }

        public decimal Balance {
            get {return balance;}
            set {balance = value;}
        }

        public DateTime CreatedAt {
            get {return createdAt;}
            set {createdAt = value;}
        }
        

    }
}