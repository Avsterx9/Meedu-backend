using Meedu.Models;

namespace Meedu.Helpers;

public static class EntityHelper
{
    public static TEntity UpdateEntity<TEntity>(TEntity baseEntity, object objectWithChanges) where TEntity : class
    {
        foreach (var property in objectWithChanges.GetType().GetProperties())
        {
            if (property.GetValue(objectWithChanges) != null)
            {
                var baseProp = baseEntity.GetType().GetProperty(property.Name);
                var newChangesProp = objectWithChanges.GetType().GetProperty(property.Name);

                if (baseProp == null || newChangesProp == null)
                    continue;

                var oldValue = baseProp.GetValue(baseEntity);
                var newValue = newChangesProp.GetValue(objectWithChanges);

                if (newValue != null && newValue.GetType() == typeof(SubjectDto))
                    continue;

                if ((oldValue == null && newValue != null) || (oldValue != null && !oldValue.Equals(newValue)))
                {
                    baseProp.SetValue(baseEntity, property.GetValue(objectWithChanges));
                }
            }
        }
        return baseEntity;
    }
}
