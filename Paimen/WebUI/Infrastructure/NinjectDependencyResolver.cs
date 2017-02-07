using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject;
using System.Linq;
using System.Web;
using LoginManagement.Dal;

namespace F1App.WebUI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            throw new NotImplementedException();
        }

        private void AddBindings()
        {
            kernel.Bind(typeof(IDao<>)).To(typeof(IDao<>));
        }
    }
}