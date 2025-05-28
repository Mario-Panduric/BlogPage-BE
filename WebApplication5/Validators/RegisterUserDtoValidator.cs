using FluentValidation;
using WebApplication5.DTOs;
namespace WebApplication5.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(p => p.UserPassword).NotEmpty().WithMessage("Your password cannot be empty")
                    .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                    .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                    .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                    .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                    .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");

            RuleFor(e => e.Email).EmailAddress().WithMessage("Email is incorrect.");
            RuleFor(u => u.UserName).NotNull().WithMessage("Username cannot be empty")
                .Matches(@"^[a-zA-Z0-9]{3,20}$").WithMessage("Username can contain only numbers and letters and must be 3-20 characters long.");
        }
    }
}
