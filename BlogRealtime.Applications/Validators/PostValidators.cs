using BlogRealtime.Domain.Dtos;
using FluentValidation;

namespace BlogRealtime.Application.Validators;

public class CreatePostValidator : AbstractValidator<CreatePostDto>
{
    public CreatePostValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(5);
        RuleFor(x => x.Body).NotEmpty().MinimumLength(10);
        RuleFor(x => x.Image).NotEmpty();
    }
}

public class UpdatePostValidator : AbstractValidator<UpdatePostDto>
{
    public UpdatePostValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(5);
        RuleFor(x => x.Body).NotEmpty().MinimumLength(10);
        RuleFor(x => x.Image).NotEmpty();
    }
}
