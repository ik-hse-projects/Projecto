using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Projecto
{
    /// <summary>
    /// Хранит список пользователй и проектов. Умеет правильно сериализовываться.
    /// </summary>
    public class State
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            ContractResolver = new WritablePropertiesOnlyResolver(),
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            TypeNameHandling = TypeNameHandling.Auto
        };

        public List<IUser> Users { get; } = new();
        public List<Project> Projects { get; } = new();

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, Settings);
        }

        public static State Deserialize(string data)
        {
            return JsonConvert.DeserializeObject<State>(data, Settings) ?? throw new InvalidDataException();
        }

        /// <summary>
        /// Мы хотим сериализовывать все поля, корме вида "public Foo Prop => CreateFoo()",
        /// т.к. очевидно, что им невозможно никакаим образом присовить значение.
        /// Этот класс позволяет это делать!
        /// </summary>
        // Основано на: https://stackoverflow.com/a/18548894 и https://stackoverflow.com/a/16506710
        private class WritablePropertiesOnlyResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                return base.CreateProperties(type, memberSerialization)
                    .Where(p =>
                    {
                        if (!p.Readable)
                        {
                            // Если поле нельзя прочесть, то зачем оно нам?
                            return false;
                        }

                        if (!p.Writable)
                        {
                            // Возможно, что оно именно того вида, который нас интересует (`Prop => ...`).
                            var isAutoProperty = fields.Any(f => f.Name.Contains($"<{p.PropertyName}>"));
                            if (!isAutoProperty)
                            {
                                // Если нет backing field, то оно точно такого вида. Отбрасываем!
                                return false;
                            }
                        }

                        // Все остальные поля пусть будут.
                        return true;
                    }).ToList();
            }
        }
    }
}