namespace RPC;

public enum Operation {
    CreateUser,
    GetAllUsers,
    GetUserByPhoneNumber,
    CreateGroup,
    GetAllGroups,
    GetGroupFromUser,
    CreateExpense,
    GetExpensesFromGroup,
    GetExpensesFromUser,
    GetExpensesFromUserInGroup
}
