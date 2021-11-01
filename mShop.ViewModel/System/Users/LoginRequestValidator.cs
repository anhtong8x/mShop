using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace mShop.ViewModel.System.Users
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("User name is required");
            RuleFor(x => x.PassWord).NotEmpty().WithMessage("PassWord is required")
                .MinimumLength(6).WithMessage("Password is at least 6 character");
        }
    }
}