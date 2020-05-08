using System.Windows;

namespace Technovert.WPF.GenericMVVM
{
    public class Screen : IViewAware {
        public Window View { get; set; }

        public void Close(bool? result) {
            View.DialogResult = result;
            View.Close();
        }
    }
}