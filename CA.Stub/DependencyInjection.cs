using System;
using CA.Stub.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace CA.Stub
{
    public static class DependencyInjection 
    {
        public static IServiceCollection AddStubs(this IServiceCollection collection)
        {
            collection.AddScoped<IStubManager, StubManager>();
            return collection;
        } 
    }
}