namespace RPC;

public enum Operation {
    CreateUser,
    GetAllUsers,
    GetUserByPhoneNumber,
    CreateGroup,
    GetAllGroups,
    GetGroupsFromUser,
    GetGroupById,
    CreateExpense,
    GetExpensesFromGroup,
    GetExpensesFromUser,
    GetExpensesFromUserInGroup,
    JoinGroup
}
