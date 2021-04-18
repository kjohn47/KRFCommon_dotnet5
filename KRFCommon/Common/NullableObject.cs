namespace KRFCommon.Common
{
    public class NullableObject<TObject>
    {
        protected NullableObject()
        {
        }

        protected NullableObject( TObject value )
        {
            this.Value = value;
        }

        public TObject Value { get; private set; }
        public bool HasValue => !( this.Value == null );

        public static NullableObject<TObject> EmptyResult()
        {
            return new NullableObject<TObject>();
        }

        public static NullableObject<TObject> FromResult( TObject result )
        {
            return new NullableObject<TObject>( result );
        }
    }
}
