using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace Technovert.WPF.GenericMVVM
{

    /// <summary>
    /// Checks Execution Mode
    /// </summary>
    public static class Execution {
        private static bool? _inDesignMode;

        /// <summary>
        /// Checks if Execution is in Design Mode
        /// </summary>
        public static bool InDesignMode {
            get {
                if (_inDesignMode == null) {
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    _inDesignMode = (bool) DependencyPropertyDescriptor.FromProperty(prop, typeof (FrameworkElement)).Metadata.DefaultValue;

                    if (!_inDesignMode.GetValueOrDefault(false) &&
                        Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal)) {
                        _inDesignMode = true;
                    }
                }

                return _inDesignMode.GetValueOrDefault(false);
            }
        }
    }
}