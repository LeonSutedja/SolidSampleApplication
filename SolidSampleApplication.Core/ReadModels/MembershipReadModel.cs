using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.Core
{
    // value object
    public class MembershipPointReadModel
    {
        public double Amount { get; private set; }
        public MembershipPointsType Type { get; private set; }
        public DateTime EarnedAt { get; private set; }

        protected MembershipPointReadModel()
        {
        }

        public MembershipPointReadModel(double amount, MembershipPointsType type, DateTime earnedAt)
        {
            Amount = amount;
            Type = type;
            EarnedAt = earnedAt;
        }
    }

    public class MembershipReadModel
    {
        public static MembershipReadModel FromAggregate(Membership aggregateMembership)
        {
            var points = aggregateMembership.Points.Select(p => new MembershipPointReadModel(p.Amount, p.Type, p.EarnedAt));
            return new MembershipReadModel(
                aggregateMembership.Id,
                aggregateMembership.Type,
                aggregateMembership.CustomerId,
                points.ToList(),
                points.Sum(p => p.Amount), aggregateMembership.Version);
        }

        // aggregate id
        public Guid Id { get; private set; }

        private List<MembershipPointReadModel> _points = new List<MembershipPointReadModel>();
        public List<MembershipPointReadModel> Points { get => _points; }

        public MembershipType Type { get; private set; }
        public Guid CustomerId { get; private set; }
        public double TotalPoints { get; private set; }
        public int Version { get; private set; }

        protected MembershipReadModel()
        {
            _points = new List<MembershipPointReadModel>();
        }

        public MembershipReadModel(
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
            _points = points ?? throw new ArgumentNullException(nameof(points));
            Version = version;
            TotalPoints = totalPoints;
        }
    }
}