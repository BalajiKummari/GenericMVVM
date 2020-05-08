using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Technovert.WPF.GenericMVVM
{
    public class Validatable : INotifyDataErrorInfo, INotifyPropertyChanged {
        private readonly Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = delegate { };

        public IEnumerable GetErrors(string propertyName) {
            return errors.ContainsKey(propertyName) ? errors[propertyName] : null;
        }

        public bool HasErrors {
            get { return errors.Count > 0; }
        }

        private void ValidateProperty(string propertyName) {
            var results = new List<ValidationResult>();
            ValidationContext context = new ValidationContext(this) {MemberName = propertyName};

            object value = GetType().GetProperty(propertyName).GetValue(this, null);

            Validator.TryValidateProperty(value, context, results);

            if (results.Any()) {
                errors[propertyName] = results.Select(c => c.ErrorMessage).ToList();
            }
            else {
                errors.Remove(propertyName);
            }
            ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            ValidateProperty(propertyName);
        }
    }
}