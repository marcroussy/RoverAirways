using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Contracts
{
    public interface IHttpTriggerService<T>
    {
        Task<HttpTriggerResult> Process(T message);
    }
}
