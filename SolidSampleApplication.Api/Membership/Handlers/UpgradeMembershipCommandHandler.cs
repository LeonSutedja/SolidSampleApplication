using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Core.Services.MembershipServices;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using SolidSampleApplication.Infrastructure.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class UpgradeMembershipCommand : ICommand<DefaultResponse>
    {
        public Guid? Id { get; init; }

        public UpgradeMembershipCommand(Guid? id)
        {
            Id = id;
        }
    }

    public class UpgradeMembershipCommandValidator : AbstractValidator<UpgradeMembershipCommand>
    {
        public UpgradeMembershipCommandValidator()
        {
            RuleFor(x => x.Id).NotNull();
        }
    }

    public class UpgradeMembershipCommandHandler : ICommandHandler<UpgradeMembershipCommand, DefaultResponse>
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly IMembershipDomainService _service;

        public UpgradeMembershipCommandHandler(ReadModelDbContext readModelDbContext, IMembershipDomainService service)
        {
            _readModelDbContext = readModelDbContext;
            _service = service;
        }

        public async Task<DefaultResponse> Handle(UpgradeMembershipCommand request, CancellationToken cancellationToken)
        {
            await _service.UpgradeMembership(request.Id.Value);

            var membership = await _readModelDbContext.Memberships.FirstOrDefaultAsync(m => m.Id == request.Id.Value);
            return DefaultResponse.Success(membership);
        }
    }
}