using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.Core
{
    // value object
    public class MembershipPointReadModel
    {
        public Guid Id { get; private set; }
        public Guid MembershipId { get; private set; }
        public double Amount { get; private set; }
        public MembershipPointsType Type { get; private set; }

        protected MembershipPointReadModel()
        {
        }

        public MembershipPointReadModel(Guid id, Guid membershipId, double amount, MembershipPointsType type)
        {
            Id = id;
            MembershipId = membershipId;
            Amount = amount;
            Type = type;
        }
    }

    public class AggregateMembershipReadModel
    {
        public static AggregateMembershipReadModel FromAggregate(AggregateMembership aggregateMembership)
        {
            var points = aggregateMembership.Points.Select(p => new MembershipPointReadModel(p.Id, p.MembershipId, p.Amount, p.Type));
            return new AggregateMembershipReadModel(
                aggregateMembership.Membership.Id,
                aggregateMembership.Membership.Type,
                aggregateMembership.Membership.CustomerId,
                points.ToList(),
                points.Sum(p => p.Amount), aggregateMembership.Version);
        }

        // aggregate id
        public Guid Id { get; private set; }

        public MembershipType Type { get; private set; }
        public Guid CustomerId { get; private set; }
        public List<MembershipPointReadModel> Points { get; private set; }
        public double TotalPoints { get; private set; }
        public int Version { get; private set; }

        protected AggregateMembershipReadModel()
        {
            Points = new List<MembershipPointReadModel>();
        }

        public AggregateMembershipReadModel(
            Guid id,
            MembershipType type,
            Guid customerId,
            List<MembershipPointReadModel> points,
            double totalPoints,
            int version)
        {
            Id = id;
            Type = type;
            CustomerId = customerId;
            Points = points ?? throw new ArgumentNullException(nameof(points));
            Version = version;
            TotalPoints = totalPoints;
        }
    }
}