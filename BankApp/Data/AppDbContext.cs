using Microsoft.EntityFrameworkCore;
using BankApp.Models;

namespace BankApp.Data {
    public class AppDbContext : DbContext {
        public DbSet<User> Users {get; set;}
        public DbSet<Card> Cards {get; set;}
        public DbSet<Account> Accounts {get; set;}
        public DbSet<Transaction> Transactions {get; set;}

        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {

        }

    }
}