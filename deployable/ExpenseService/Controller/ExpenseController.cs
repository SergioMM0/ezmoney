using Messages.Expense.Dto;
using Messages.Expense.Request;
using Messages.Expense.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RPC;

namespace ExpenseService.Controller;

[ApiController]
[Route("expense")]
public class ExpenseController : ControllerBase {

    private readonly RpcClient _rpcClient;

    public ExpenseController(RpcClient rpcClient) {
        _rpcClient = rpcClient;
    }

    /// <summary>
    /// Retrieves all the expenses from a user in a group. Returns an empty list if no expenses were found.
    /// <br/>- Sends a request through RPC to:
    /// <br/>- ExpenseRepository
    /// </summary>
    /// <param name="groupId"><see cref="int"/></param>
    /// <param name="userId"><see cref="int"/></param>
    /// <returns>A <see cref="IActionResult"/> containing an <see cref="List{T}"/> of <see cref="ExpenseResponse"/> objects.</returns>
    [HttpGet("{groupId}/user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ExpenseResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof (string))]
    public async Task<ActionResult<List<ExpenseResponse>>> GetExpensesFromUser([FromRoute] int groupId, [FromRoute] int userId) {
        try {
            var request = new GetExpensesUserReq() {
                UserId = userId,
                GroupId = groupId
            };
            var response = await _rpcClient.CallAsync(Operation.GetExpensesFromUserInGroup, request);
            var expenses = JsonConvert.DeserializeObject<List<ExpenseResponse>>(response);
            return Ok(expenses);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error getting expenses from user in group");
        }
    }

    /// <summary>
    /// Retrieves all the expenses from a group. Returns an empty list if no expenses were found.
    /// <br/>- Sends a request through RPC to:
    /// <br/>- ExpenseRepository
    /// </summary>
    /// <param name="groupId"><see cref="int"/></param>
    /// <returns>A <see cref="IActionResult"/> containing an <see cref="List{T}"/> of <see cref="ExpenseResponse"/> objects.</returns>
    [HttpGet("{groupId}/expenses")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ExpenseResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof (string))]
    public async Task<ActionResult<List<ExpenseResponse>>> GetExpensesFromGroup([FromRoute] int groupId) {
        try {
            var request = new GetExpensesReq() {
                GroupId = groupId
            };
            var response = await _rpcClient.CallAsync(Operation.GetExpensesFromGroup, request);
            var expenses = JsonConvert.DeserializeObject<List<ExpenseResponse>>(response);
            return Ok(expenses);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error getting expenses from group");
        }
    }

    /// <summary>
    /// Creates an expense.
    /// <br/>- Sends a request through RPC to:
    /// <br/>- ExpenseRepository
    /// </summary>
    /// <param name="request"><see cref="PostExpense"/></param>
    /// <returns><see cref="ExpenseResponse"/></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ExpenseResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof (string))]
    public async Task<ActionResult<ExpenseResponse>> Create([FromBody] PostExpense request) {
        try {
            // Add datetime to the request
            var expenseRequest = new CreateExpenseReq {
                Amount = request.Amount,
                Description = request.Description,
                GroupId = request.GroupId,
                OwnerId = request.OwnerId,
                Participants = request.Participants,
                Date = request.Date
            };

            var response = await _rpcClient.CallAsync(Operation.CreateExpense, expenseRequest);
            var expense = JsonConvert.DeserializeObject<ExpenseResponse>(response);
            return Ok(expense);
        } catch (Exception e) {
            Console.WriteLine(e);
            return BadRequest("Error creating expense");
        }
    }
}
