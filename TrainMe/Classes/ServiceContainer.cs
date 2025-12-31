using System;
using System.Collections.Generic;

namespace TrainMe.Classes {
    public class ServiceContainer {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public static void Register<T>(T service) {
            _services[typeof(T)] = service;
        }

        public static T Get<T>() {
            return (T)_services[typeof(T)];
        }
    }
}
