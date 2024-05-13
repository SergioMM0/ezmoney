using FluentValidation;
using Messages.Group.Dto;

namespace Messages.Group;

/// <summary>
/// DTO for user joining a group
/// </summary>
public class JoinGroupReq {
    /// <summary>
    /// The id of the user that wants to join the group
    /// </summary>
    public required int UserId { get; set; }
    
    /// <summary>
    /// The token of the group that the user wants to join
    /// </summary>
    public required string Token { get; set; }
}

public class JoinGroupReqValidator : AbstractValidator<JoinGroupReq> {
    public JoinGroupReqValidator() {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0");

        RuleFor(x => x.Token)
            .NotEmpty()
            .Length(36)
            .WithMessage("Invalid token");
    }
}
