using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Core;
using SolidSampleApplication.Core.Services.MembershipServices;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using SolidSampleApplication.Infrastructure.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class EarnPointsAggregateMembershipCommand : ICommand<DefaultResponse>
    {
        public Guid? Id { get; init; }

        public double? Points { get; init; }

        public MembershipPointsType? Type { get; init; }

        public EarnPointsAggregateMembershipCommand(Guid? id, MembershipPointsType? type, double? points)
        {
            Id = id;
            Type = type;
            Points = points;
        }
    }

    public class EarnPointsAggregateMembershipCommandValidator : AbstractValidator<EarnPointsAggregateMembershipCommand>
    {
        public EarnPointsAggregateMembershipCommandValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Points).NotNull();
            RuleFor(x => x.Type).NotNull();
        }
    }

    public class EarnPointsAggregateMembershipCommandHandler : ICommandHandler<EarnPointsAggregateMembershipCommand, DefaultResponse>
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly IMembershipDomainService _service;

        public EarnPointsAggregateMembershipCommandHandler(ReadModelDbContext readModelDbContext, IMembershipDomainService service)
        {
            _readModelDbContext = readModelDbContext;
            _service = service;
        }

        public async Task<DefaultResponse> Handle(EarnPointsAggregateMembershipCommand request, CancellationToken cancellationToken)
        {
            await _service.PointsEarned(request.Id.Value, request.Points.Value, request.Type.Value);

            var membership = await _readModelDbContext.Memberships.FirstOrDefaultAsync(m => m.Id == request.Id.Value);
            return DefaultResponse.Success(membership);
        }
    }
}