using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Projecto
{
    /// <summary>
    /// Хранит список пользователй и проектов. Умеет правильно сериализовываться.
    /// </summary>
    public class State
    {
        public List<IUser> Users { get; private set; } = new();
        public List<Project> Projects { get; private set; } = new();

        private static readonly JsonSerializerSettings Settings = new()
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            TypeNameHandling = TypeNameHandling.Auto,
        };

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, Settings);
        }

        public static State Deserialize(string data)
        {
            return JsonConvert.DeserializeObject<State>(data, Settings) ?? throw new InvalidDataException();
        }
    }
}