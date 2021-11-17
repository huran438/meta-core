using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;

namespace Meta.Core.Runtime
{
    public abstract class MDatabase : ScriptableObject
    {
        public ref Dictionary<string, string> GuidPathPairs => ref guidPathPairs;

        public string Guid => guid;

        [SerializeField, HideInInspector]
        private string guid = MetaConstants.GUID;

        [OdinSerialize]
        private Dictionary<string, string> guidPathPairs = new Dictionary<string, string>();

        private void Awake()
        {
            if (guid == string.Empty || guid == MetaConstants.GUID)
                guid = System.Guid.NewGuid().ToString();
        }

        protected abstract void GenerateGuidPathPairs(ref Dictionary<string, string> value);

        private void OnValidate()
        {
            guidPathPairs.Clear();
            GenerateGuidPathPairs(ref guidPathPairs);
        }
    }
}