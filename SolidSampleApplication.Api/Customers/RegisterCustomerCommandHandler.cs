using FluentValidation;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Core.Services.CustomerServices;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using SolidSampleApplication.Infrastructure.Shared;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Customers
{
    public class RegisterCustomerCommand : ICommand<DefaultResponse>
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
    {
        public RegisterCustomerCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotNull()
                .MinimumLength(3)
                .MaximumLength(20);
            RuleFor(x => x.FirstName)
                .NotNull()
                .MinimumLength(3)
                .MaximumLength(50);
            RuleFor(x => x.LastName)
                .NotNull()
                .MinimumLength(3)
                .MaximumLength(50);
            RuleFor(x => x.Email)
                .NotNull()
                .EmailAddress();
        }
    }

    public class RegisterCustomerCommandHandler : ICommandHandler<RegisterCustomerCommand, DefaultResponse>
    {
        private readonly ReadModelDbContext _context;
        private readonly ICustomerDomainService _service;

        public RegisterCustomerCommandHandler(ReadModelDbContext context, ICustomerDomainService service)
        {
            _context = context;
            _service = service;
        }

        public async Task<DefaultResponse> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
        {
            var isSuccess = await _service.RegisterCustomerAsync(request.Username, request.FirstName, request.LastName, request.Email);
            if(!isSuccess)
                return DefaultResponse.Failed(request, "Username exists");

            var customer = _context.Customers.FirstOrDefault(c => c.Username == request.Username);
            return DefaultResponse.Success(customer);
        }
    }
}