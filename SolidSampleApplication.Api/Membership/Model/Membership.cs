using System;

namespace SolidSampleApplication.Api.Membership
{
    public class Membership
    {
        public static Membership New(MembershipType type, string username)
        {
            return new Membership(Guid.NewGuid(), type, username);
        }

        public Guid Id { get; private set; }
        public MembershipType Type { get; private set; }
        public string Username { get; private set; }

        protected Membership(Guid id, MembershipType type, string username)
        {
            Id = id;
            Type = type;
            Username = username;
        }
    }
}