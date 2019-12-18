using KRFCommon.CQRS.Common;

namespace KRFCommon.CQRS.Query
{
    public class QueryOut<Toutput> : IQueryOut<Toutput> where Toutput : class
    {
        protected QueryOut(Toutput result)
        {
            this.Result = result;
        }
        protected QueryOut(ErrorOut error, bool hasError)
        {
            this.HasError = hasError;
            this.Error = error;
        }

        public Toutput Result { get; }
        public ErrorOut Error { get; }
        public bool HasError { get; }

        public static QueryOut<Toutput> GenerateFault(ErrorOut error)
        {
            return new QueryOut<Toutput>(error, true);
        }

        public static QueryOut<Toutput> GenerateResult(Toutput result)
        {
            return new QueryOut<Toutput>(result);
        }
    }
}
