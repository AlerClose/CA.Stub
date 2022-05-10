using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CA.Stub.Contract;

namespace CA.Stub
{
    public interface IStubStore
    {
        Task<int> Update(TemplateItem item);
        Task Remove(int id);

        Task<IEnumerable<TemplateItem>> Find(Func<TemplateItem, bool> query);
    }
}