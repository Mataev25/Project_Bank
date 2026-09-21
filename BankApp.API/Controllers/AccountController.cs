using Microsoft.AspNetCore.Mvc;
using BankApp.Services;
using BankApp.Models;
using BankApp.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace BankApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase {
    private readonly AccountService accService;

    public AccountController() {
        var context = new AppDbContext();
        accService = new AccountService(context);
    }

    private int GetCurrentUserId() {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("Пользователь не авторизован");
        return int.Parse(userIdClaim.Value);
    }

    [HttpGet("balance")]
    public IActionResult GetBalance () {
        int userId = GetCurrentUserId();
        var balance = accService.GetBalance(userId);
        return Ok(new {Balance = balance});
    }

    [HttpPost("deposit")]
    public IActionResult Deposit([FromBody] TransactionRequest request) {
        int userId = GetCurrentUserId();
        var result = accService.Deposit(userId, request.Amount);
        if (!result) 
            return BadRequest("Не удалось пополнить счет. Проверьте сумму.");
        return Ok(new {Message = "Счет пополнен", NewBalance = accService.GetBalance(userId)});
    }

    [HttpPost("withdraw")]
    public IActionResult Withdraw([FromBody] TransactionRequest request) {
        int userId = GetCurrentUserId();
        var result = accService.Withdraw(userId, request.Amount);
        if (!result)
            return BadRequest("Не удалось снять средства. Проверьте сумму или баланс.");
        return Ok(new {Message = "Средства сняты", NewBalance = accService.GetBalance(userId)});
    }

    [HttpGet("transactions")]
    public IActionResult GetTransactions() {
        int userId = GetCurrentUserId();
        var transactions = accService.GetTransactionsByUserId(userId);
        return Ok(transactions);
    }
}

public class TransactionRequest {
    public decimal Amount {get; set;}
}