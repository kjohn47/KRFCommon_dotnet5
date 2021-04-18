namespace KRFCommon.CQRS.Common
{
    using KRFCommon.Common;

    public class ResponseOut<Toutput> : IResponseOut<Toutput> where Toutput : ICQRSResponse
    {
        protected ResponseOut( Toutput result )
        {
            this.Error = NullableObject<ErrorOut>.EmptyResult();
            this.Result = result;
        }
        protected ResponseOut( ErrorOut error )
        {
            this.Error = NullableObject<ErrorOut>.FromResult( error );
        }

        public Toutput Result { get; }
        public NullableObject<ErrorOut> Error { get; }

        public static ResponseOut<Toutput> GenerateFault( ErrorOut error )
        {
            return new ResponseOut<Toutput>( error );
        }

        public static ResponseOut<Toutput> GenerateResult( Toutput result )
        {
            return new ResponseOut<Toutput>( result );
        }
    }
}
