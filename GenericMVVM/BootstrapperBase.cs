using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Technovert.WPF.GenericMVVM
{
    public abstract class BootstrapperBase {

        /// <summary>
        /// Specify the list of Assemblies to search for Views or ViewModels
        /// </summary>
        /// <remarks> Make sure to follow MVVM Naming Convention </remarks>
        /// <returns> list of Assemblies </returns>
        protected virtual IEnumerable<Assembly> SelectAssemblies() {
            if (Execution.InDesignMode) {
                var appDomain = AppDomain.CurrentDomain;
                var assemblies = appDomain.GetType()
                    .GetMethod("GetAssemblies")
                    .Invoke(appDomain, null) as Assembly[] ?? new Assembly[] {};

                var applicationAssembly = assemblies.LastOrDefault(ContainsApplicationClass);
                return applicationAssembly == null ? new Assembly[] {} : new[] {applicationAssembly};
            }
            var entryAssembly = Assembly.GetEntryAssembly();
            return entryAssembly == null ? new Assembly[] {} : new[] {entryAssembly};
        }

        /// <summary>
        ///  Specify special View Model Mappings that not follows the Naming Convention
        /// </summary>
        /// <returns> Dictionary With View Model as Key and View as Value </returns>
        protected virtual Dictionary<Type, Type> SpecialViewModelMappings() {
            return null;
        }

        private static bool ContainsApplicationClass(Assembly assembly) {
            var containsApp = false;

            try {
                containsApp = assembly.EntryPoint != null &&
                              assembly.GetExportedTypes().Any(t => t.IsSubclassOf(typeof (Application)));
            }
            catch {}
            return containsApp;
        }

        /// <summary>
        /// Call this method In the Application entry Point to Configure
        /// </summary>
        public void Start() {

            //loads the Specifies Assemblies for Searching ViewModels or Views
            AssemblySource.Instance.AddRange(SelectAssemblies());

            // Loads any special ViewModel Mappings that Dosent Follow the Naming Convention
            ViewModelMappings.Mappings = SpecialViewModelMappings();

            // COnfigure the IOC for the Corresponding Mode
            if (Execution.InDesignMode) {
                
                ConfigureForDesignTime();
            }
            else {
               
                ConfigureForRuntime();
            }
            IoC.GetInstance = GetInstance;
        }

        /// <summary>
        /// Initialize the IoC Container Here
        /// </summary>
        protected virtual void ConfigureForRuntime() {}

        /// <summary>
        /// Initialize the IoC Container Here for Design time 
        /// </summary>
        protected virtual void ConfigureForDesignTime() {}

        /// <summary>
        ///  Resolves the Requested Type from the container
        /// </summary>
        /// <returns>object of the requested type</returns>
        protected abstract object GetInstance(Type service, string key);
    }
}