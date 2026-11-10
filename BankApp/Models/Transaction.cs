using System;

namespace BankApp.Models {
    public class Transaction {
        private int id;
        private int userId;
        private string type;
        private decimal amount;
        private DateTime date;
        private string description;

        public Transaction(int userId, string type, decimal amount, string description = "") {
            this.id = 0;
            this.userId = userId;
            this.type = type;
            this.amount = amount;
            this.date = DateTime.UtcNow;
            this.description = description;
        }

        public int Id {
            get {return id;}
            set {id = value;}
        }

        public int UserId {
            get {return userId;}
            set {userId = value;}
        }

        public string Type {
            get {return type;}
            set {type = value;}
        }

        public decimal Amount {
            get {return amount;}
            set {amount = value;}
        }

        public DateTime Date {
            get {return date;}
            set {date = value;}
        }

        public string Description {
            get {return description;}
            set {description = value;}
        }
    }
}