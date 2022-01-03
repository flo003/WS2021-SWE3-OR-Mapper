using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS2021.SWE3.OR_Mapper.ModelEntities;

namespace WS2021.SWE3.OR_Mapper
{
    public class DbChache
    {
        private static Dictionary<Type, Dictionary<object, object>> _cache = new();
        private EntityRegistry _entityRegistry;
        public DbChache(EntityRegistry entityRegistry)
        {
            _entityRegistry = entityRegistry;
        }

        public void StoreValue(object value)
        {
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(value);
            if (_cache.ContainsKey(value.GetType()))
            {
                _cache[value.GetType()][modelEntity.PrimaryKey.GetValue(value)] = value;
            }
            else
            {   
                _cache[value.GetType()] = new Dictionary<object, object>() { { modelEntity.PrimaryKey.GetValue(value), value } };
            }
        }

        public void RemoveValue(object value)
        {
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(value);
            if (_cache.ContainsKey(value.GetType()))
            {
                _cache[value.GetType()].Remove(modelEntity.PrimaryKey.GetValue(value));
            }
        }

        public object GetValue(Type type, object primaryKey)
        {
            if (_cache.ContainsKey(type) && _cache[type].ContainsKey(primaryKey))
            {
                return _cache[type][primaryKey];
            }
            return null;
        }
    }
}
