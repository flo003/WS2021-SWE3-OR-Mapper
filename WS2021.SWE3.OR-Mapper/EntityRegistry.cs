using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS2021.SWE3.OR_Mapper.ModelEntities;

namespace WS2021.SWE3.OR_Mapper
{
    public class EntityRegistry
    {
        private static Dictionary<Type, ModelEntity> entities = new Dictionary<Type, ModelEntity>();
        internal ModelEntity GetModelEntity(object obj)
        {
            return GetModelEntity(obj.GetType());
        }
        internal ModelEntity GetModelEntity(Type type)
        {
            if (entities.ContainsKey(type))
            {
                return entities[type];
            }
            ModelEntity modelEntity = new ModelEntity(type);
            entities.Add(type, modelEntity);
            return modelEntity;
        }
    }
}
