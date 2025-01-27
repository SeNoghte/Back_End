using Application.Common.Models;

namespace Test.Application
{
    public class TestHandler
    {

        public async Task<bool> HandleAfter(object x)
        {
            return true;    
        }

        public async Task<T> Handle<T>(T obj)
        {
            return obj;
        }
    }
}
