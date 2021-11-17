using System;
using UnityEngine;

namespace Meta.Core.Runtime
{
    [Serializable]
    public abstract class MetaContainer : IMContainer
    {
        [SerializeField, HideInInspector]
        private string guid = System.Guid.NewGuid().ToString();

        [SerializeField]
        private string name = string.Empty;
        
        public string Guid => guid;
        public string Name => name;
    }
}