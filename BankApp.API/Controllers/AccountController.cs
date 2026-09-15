using Microsoft.AspNetCore.Mvc;
using BankApp.Services;
using BankApp.Models;
using BankApp.Data;

namespace BankApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase {
    private readonly AccountService accService;

    public AccountController() {
        var context = new AppDbContext();
        accService = new AccountService(context);
    }

    [HttpGet("{userId}/balance")]
    public IActionResult GetBalance (int userId) {
        var balance = accService.GetBalance(userId);
        return Ok(new {UserId = userId, Balance = balance});
    }

    [HttpPost("{userId}/deposit")]
    public IActionResult Deposit(int userId, [FromBody] TransactionRequest request) {
        var result = accService.Deposit(userId, request.Amount);
        if (!result) 
            return BadRequest("Не удалось пополнить счет. Проверьте сумму.");
        return Ok(new {Message = "Счет пополнен", NewBalance = accService.GetBalance(userId)});
    }

    [HttpPost("{userId}/withdraw")]
    public IActionResult Withdraw(int userId, [FromBody] TransactionRequest request) {
        var result = accService.Withdraw(userId, request.Amount);
        if (!result)
            return BadRequest("Не удалось снять средства. Проверьте сумму или баланс.");
        return Ok(new {Message = "Средства сняты", NewBalance = accService.GetBalance(userId)});
    }

    [HttpGet("{userId}/transactions")]
    public IActionResult GetTransactions(int userId) {
        var transactions = accService.GetTransactionsByUserId(userId);
        return Ok(transactions);
    }
}

public class TransactionRequest {
    public decimal Amount {get; set;}
}