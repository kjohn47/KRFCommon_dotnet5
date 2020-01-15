﻿using KRFCommon.CQRS.Common;
using System.Threading.Tasks;

namespace KRFCommon.CQRS.Query
{
    public interface IQuery<in Tinput, Toutput>
        where Tinput : class
        where Toutput : class
    {
        Task<IResponseOut<Toutput>> QueryAsync(Tinput request);
    }
}
