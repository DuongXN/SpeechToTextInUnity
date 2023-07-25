using System;
using System.Collections.Generic;

namespace FrostweepGames.Plugins.GoogleCloud.Translation
{
    public class ServiceLocatorTranslation : IDisposable
    {
        private Dictionary<Type, IServiceTranslation> _services;

        internal ServiceLocatorTranslation()
        {
            _services = new Dictionary<Type, IServiceTranslation>();
            AddService<ITranslationManager>(new TranslationManager());
        }

        public void InitServices()
        {
            foreach (var service in _services)
                service.Value.Init();
        }

        public void Update()
        {
            foreach (var service in _services)
                service.Value.Update();
        }

        public void Dispose()
        {
            foreach (var service in _services)
                service.Value.Dispose();
        }

        public T Get<T>()
        {
            if (_services.ContainsKey(typeof(T)))
                return (T)_services[typeof(T)];
            else
                throw new NotImplementedException(typeof(T) + " not implemented!");
        }

        private void AddService<T>(IServiceTranslation service)
        {
            _services.Add(typeof(T), service);
        }
    }
}