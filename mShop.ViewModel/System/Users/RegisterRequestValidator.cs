using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.ViewModel.System.Users
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName is required")
                .MaximumLength(200).WithMessage("First name cannot over 200 character");

            RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is required")
                 .MaximumLength(200).WithMessage("LastName cannot over 200 character");

            RuleFor(x => x.Dob).GreaterThan(DateTime.Now.AddYears(-100)).WithMessage("Brith day cannot greater than 100 year");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("email formart is not match");

            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required");

            RuleFor(x => x.UserName).NotEmpty().WithMessage("User name is required");
            RuleFor(x => x.Password).NotEmpty().WithMessage("PassWord is required")
                .MinimumLength(6).WithMessage("Password is at least 6 character");
            RuleFor(x => x).Custom((request, context) =>
            {
                if (request.Password != request.ConfirmPassword)
                    context.AddFailure("confirm password is not match")
            });
        }
    }
}