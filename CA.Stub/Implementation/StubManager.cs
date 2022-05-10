using System.Linq;
using System.Threading.Tasks;
using CA.Stub.Contract;
using Newtonsoft.Json;

namespace CA.Stub.Implementation
{
    class StubManager: IStubManager
    {
        private readonly IStubStore _stubStore;

        public StubManager(IStubStore stubStore)
        {
            _stubStore = stubStore;
        }
        
        
        public async Task<int> UpdateMapping(string condition, object result)
        {
            var itemList = await _stubStore.Find(a => a.Condition == condition);
            var item = itemList.FirstOrDefault() ?? new TemplateItem()
            {
                Condition = condition,
                Template = JsonConvert.SerializeObject(result)
            };
            return await _stubStore.Update(item);
        }
        
        public async Task EditMapping(int id, object result)
        {
            var itemList = await _stubStore.Find(a => a.Id == id);
            var item = itemList.Single();
            item.Template = JsonConvert.SerializeObject(result);
            await _stubStore.Update(item);
        }

        public Task RemoveMapping(int id)
        {
            return _stubStore.Remove(id);
        }

        public async Task RemoveMapping(string condition)
        {
            var itemList = await _stubStore.Find(a => a.Condition == condition);
            var item = itemList.FirstOrDefault();
            if (item == null)
                return;
            
            await _stubStore.Remove(item.Id);
        }
        
        public async Task<object> Resolve(object input)
        {
            var templates = await _stubStore.Find(t => t.Template != null);
            foreach (var template in templates)
            {
                if (IsAssert(template.Condition, input))
                    return PrepareStub(input, template);
            }

            return null;
        }

        private object PrepareStub(object input, TemplateItem template)
        {
            var result = JsonConvert.DeserializeObject(template.Template);
            return result;
        }

        internal bool IsAssert(string condition, object input)
        {
            var conditionPaths = condition.Split('=');
            if (conditionPaths.Length != 2)
                return false;
            
            string pathPart = conditionPaths[0].Trim();
            string valuePart = conditionPaths[1].Trim();

            return valuePart == GetValuePart(pathPart.Split('.'), input);
        }

        private string GetValuePart(string[] path, object input)
        {
            var propertyName = path[0];
            var property = input.GetType().GetProperty(propertyName);
            if (property == null)
                return null;

            var propertyValue = property.GetValue(input);
            if (propertyValue == null)
                return null;
            
            if (path.Length == 1)
                return propertyValue?.ToString();

            var pathFromChild = path.Skip(1);
            return GetValuePart(pathFromChild.ToArray(), propertyValue);
        }
    }
}