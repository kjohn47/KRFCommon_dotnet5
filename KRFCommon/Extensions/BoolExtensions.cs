namespace KRFCommon.Extensions
{
    public static class BoolExtensions
    {
        public static bool? GetTrueOrNull( this bool? condition )
        {
            if ( condition ?? false )
            {
                return true;
            }

            return null;
        }

        public static bool? GetTrueOrNull( this bool condition )
        {
            if ( condition )
            {
                return true;
            }

            return null;
        }
    }
}