using System;
using UnityEngine;

namespace Meta.Core.Runtime
{
    [Serializable]
    public abstract class MetaRef
    {
        public string DatabaseGuid => _databaseGuid;
        public string DatabaseName => _databaseName;
        public string Guid => _guid;
        public string Name => _name;

        [SerializeField, HideInInspector]
        private string _databaseGuid;

        [SerializeField, HideInInspector]
        private string _databaseName;

        [SerializeField, HideInInspector]
        private string _guid;
        
        [SerializeField, HideInInspector]
        private string _name;

        protected MetaRef(string databaseGuid, string databaseName, string guid, string name)
        {
            _databaseGuid = databaseGuid;
            _databaseName = databaseName;
            _guid = guid;
            _name = name;
        }

        protected MetaRef()
        {
            _guid = string.Empty;
            _databaseName = string.Empty;
            _databaseGuid = string.Empty;
            _name = string.Empty;
        }

        protected bool Equals(MetaRef other)
        {
            return Equals(Guid, other.Guid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (obj.GetType() != GetType()) return false;
            return Equals((MetaRef)obj);
        }

        public override int GetHashCode()
        {
            return _guid == MetaConstants.GUID ? 0 : _guid.GetHashCode();
        }

        public override string ToString()
        {
            return _name;
        }

        public static bool operator ==(MetaRef a, MetaRef b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            return a.Guid.Equals(b.Guid);
        }

        public static bool operator !=(MetaRef a, MetaRef b)
        {
            return !(a == b);
        }

        public bool IsNone => Guid == MetaConstants.GUID;
    }
}