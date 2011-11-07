namespace Mod05_ChelasDAL.Mappers
{
    using System;
    using System.Collections.Generic;

    public class IdentityMap
    {
        private readonly Dictionary<Type, Dictionary<string, object>> _cache = new Dictionary<Type, Dictionary<string, object>>();

        public object TryToFind(Type type, object id)
        {
            if (!this._cache.ContainsKey(type)) return null;

            string idAsString = id.ToString();
            if (!this._cache[type].ContainsKey(idAsString)) return null;

            return this._cache[type][idAsString];
        }

        public void Store(Type type, object id, object entity)
        {
            if (!this._cache.ContainsKey(type)) {
                this._cache.Add(type, new Dictionary<string, object>());
            }
            this._cache[type][id.ToString()] = entity;
        }

        public void ClearAll()
        {
            this._cache.Clear();
        }

        public void RemoveAllInstancesOf(Type type)
        {
            if (this._cache.ContainsKey(type))
            {
                this._cache.Remove(type);
            }
        }

        public void Remove(object entity)
        {
            var type = entity.GetType();

            if (!this._cache.ContainsKey(type)) return;

            string keyToRemove = null;

            foreach (var pair in this._cache[type])
            {
                if (pair.Value == entity)
                {
                    keyToRemove = pair.Key;
                }
            }

            if (keyToRemove != null)
            {
                this._cache[type].Remove(keyToRemove);
            }
        }
    }
}