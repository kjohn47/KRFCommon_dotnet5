namespace KRFCommon.CQRS.Common
{
    public class ResponseOut<Toutput> : IResponseOut<Toutput> where Toutput : ICQRSResponse
    {
        protected ResponseOut(Toutput result)
        {
            this.Result = result;
        }
        protected ResponseOut(ErrorOut error)
        {
            this.Error = error;
        }

        public Toutput Result { get; }
        public ErrorOut Error { get; }

        public static ResponseOut<Toutput> GenerateFault(ErrorOut error)
        {
            return new ResponseOut<Toutput>(error);
        }

        public static ResponseOut<Toutput> GenerateResult(Toutput result)
        {
            return new ResponseOut<Toutput>(result);
        }
    }
}
