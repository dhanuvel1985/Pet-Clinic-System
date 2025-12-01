using FluentValidation;
using PetService.Application.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetService.Application.Validators
{
    public class CreatePetValidator : AbstractValidator<CreatePetCommand>
    {
        public CreatePetValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Species).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Breed).MaximumLength(100);
            RuleFor(x => x.Age).InclusiveBetween(0, 100);
        }
    }
}
