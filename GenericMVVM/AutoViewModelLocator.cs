using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Technovert.WPF.GenericMVVM
{
    /// <summary>
    /// Connects a View to its Corresponding ViewModel
    /// </summary>
    public static class AutoViewModelLocator {
        public static bool GetAutoAttachViewModel(DependencyObject obj) {
            return (bool) obj.GetValue(AutoAttachViewModelProperty);
        }

        public static void SetAutoAttachViewModel(DependencyObject obj, bool value) {
            obj.SetValue(AutoAttachViewModelProperty, value);
        }

        /// <summary>
        /// Dependency property
        /// Set True to Auto Attach ViewModel to That View
        /// </summary>
        public static readonly DependencyProperty AutoAttachViewModelProperty =
            DependencyProperty.RegisterAttached("AutoAttachViewModel",
                typeof (bool), typeof (AutoViewModelLocator),
                new PropertyMetadata(false, AutoAttachViewModelChanged));
        
        /// <summary>
        /// Will be called every time the Auto Attached View Model Changes
        /// </summary>
        private static void AutoAttachViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            
            if (DesignerProperties.GetIsInDesignMode(d)) {
                //Do noting in DesignMode
                return;
            }

            Type viewType = d.GetType();

            // Replaces the Name 'View' with 'ViewModel'
            string viewModelTypeName = viewType.FullName.Replace("View", "ViewModel");  

            // the ViewModel will be searched in these assemblies
            var assembliesForSearchingIn = AssemblySource.Instance; 

            var allExportedTypes = new List<Type>();
            //Get All public types/ViewModels present in the assemblies
            foreach (var assembly in assembliesForSearchingIn) {
                allExportedTypes.AddRange(assembly.GetExportedTypes());
            }

            // Get Corresponding ViewModel Type 
            Type viewModelType = allExportedTypes.Single(x => x.FullName == viewModelTypeName);
            
            // Create a New Instance of the Matched View Model
            object viewModel = IoC.GetInstance(viewModelType, null);

            //Set the DataContext of the View with the ViewModel Found
            ((FrameworkElement) d).DataContext = viewModel;
        }
    }
}