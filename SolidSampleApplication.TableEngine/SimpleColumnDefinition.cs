namespace SolidSampleApplication.TableEngine
{
    public class SimpleColumnDefinition
    {
        public static SimpleColumnDefinition SimpleInt(string heading, string code, int position)
            => new SimpleColumnDefinition(heading, code, "int", position);

        public static SimpleColumnDefinition SimpleString(string heading, string code, int position)
            => new SimpleColumnDefinition(heading, code, "string", position);

        public static SimpleColumnDefinition SimpleDate(string heading, string code, int position)
            => new SimpleColumnDefinition(heading, code, "date", position);

        public string Heading { get; private set; }
        public string Code { get; private set; }
        public string Type { get; private set; }
        public int Position { get; private set; }

        private SimpleColumnDefinition(string heading, string code, string type, int position)
        {
            Heading = heading;
            Code = code;
            Type = type;
            Position = position;
        }
    }
}