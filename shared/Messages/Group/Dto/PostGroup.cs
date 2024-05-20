using FluentValidation;

namespace Messages.Group.Dto;

/// <summary>
/// Clients request to create a group
/// </summary>
public class PostGroup {
    /// <summary>
    /// Id of the user that owns the <c>Group</c>
    /// </summary>
    public required int OwnerId { get; set; }
    
    /// <summary>
    /// Name of the <c>Group</c>
    /// </summary>
    public required string Name { get; set; }
}

public class PostGroupValidator : AbstractValidator<PostGroup> {
    public PostGroupValidator() {
        RuleFor(x => x.OwnerId)
            .GreaterThan(0)
            .WithMessage("Owner ID must be greater than 0");

        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(1, 100)
            .WithMessage("Name must be between 1 and 100 characters long");
    }
}
