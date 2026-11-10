using System;
using BankApp.Models;

namespace BankApp.Data {
    public static class DbInitializer {
        public static void Seed(AppDbContext context) {
            bool hasUsers = false;
            foreach (var u in context.Users) {
                hasUsers = true;
                break;
            }
            if (hasUsers) return;

            User user1 = new User();
            user1.FullName = "Матаев Игорь";
            user1.Phone = "+79991112233";
            user1.IsFirstLogin = false;
            user1.PinHash = "5f395d07369071a505ef926527de2ac53e8c29e103dc63398315bc276224b81a";
            user1.CreatedAt = DateTime.UtcNow;

            User user2 = new User();
            user2.FullName = "Петухов Алексей";
            user2.Phone = "+79991122233";
            user2.IsFirstLogin = false;
            user2.PinHash = "3f95b1b8a32c2c0251dfdbc3c8a30aab6d6e680cf0ef03e8af84a65dff0c4a85";
            user2.CreatedAt = DateTime.UtcNow;

            User user3 = new User();
            user3.FullName = "Слинкина Юля";
            user3.Phone = "+79981112233";
            user3.IsFirstLogin = false;
            user3.PinHash = "3e7c3d4cf5be91eca62500b41e292c3474ecc0329156d11f682b609c42828df7";
            user3.CreatedAt = DateTime.UtcNow;

            context.Users.Add(user1);
            context.Users.Add(user2);
            context.Users.Add(user3);
            context.SaveChanges();

            Card card1 = new Card();
            card1.Number = "1234-5678-9010-1112";
            card1.UserId = user1.Id;
            card1.IsActive = true;
            card1.FailedAttempts = 0;
            card1.IsBlocked = false;

            Card card2 = new Card();
            card2.Number = "2345-6789-1011-1213";
            card2.UserId = user3.Id;
            card2.IsActive = true;
            card2.FailedAttempts = 0;
            card2.IsBlocked = false;

            Card card3 = new Card();
            card3.Number = "3456-7891-0111-2131";
            card3.UserId = user2.Id;
            card3.IsActive = true;
            card3.FailedAttempts = 0;
            card3.IsBlocked = false;

            context.Cards.Add(card1);
            context.Cards.Add(card2);
            context.Cards.Add(card3);
            context.SaveChanges();

            Account acc1 = new Account(user1.Id, "40817810000000000001");
            acc1.Balance = 44023.1m;

            Account acc2 = new Account(user2.Id, "40817810000000000002");
            acc2.Balance = 0m;

            Account acc3 = new Account(user3.Id, "40817810000000000003");
            acc3.Balance = 10000m;

            context.Accounts.Add(acc1);
            context.Accounts.Add(acc2);
            context.Accounts.Add(acc3);
            context.SaveChanges();



        }
    }
}