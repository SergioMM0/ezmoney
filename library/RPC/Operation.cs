namespace RPC;

public enum Operation {
    CreateUser,
    GetAllUsers,
    GetUserByPhoneNumber,
    CreateGroup,
    GetAllGroups,
    GetGroupsFromUser,
    CreateExpense,
    GetExpensesFromGroup,
    GetExpensesFromUser,
    GetExpensesFromUserInGroup
}
