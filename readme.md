
# GenericMVVM

A Micro MVVM Framework for WPF .NET Framework

## Installation

Use the  [nuget package manager](https://pip.pypa.io/en/stable/) to install GenericMVVM.

```bash
Install-Package Technovert.WPF.GenericMVVM 
```
Add reference to the package
```cs
using Technovert.WPF.GenericMVVM 
```

## Overview
This Micro Framework provides the Following Functionalities to build lightweight and scalable WPF windows applications.

The Framework offers the following functionalities:
- Bootstrapper Base for configuring services
- IoC Container Support
- Auto ViewModel Locator 
- Custom ViewModel to View Mapper
- EventAggregator for Implementing Messagebus
- RelayCommand for delegating ICommand's with predicate
- Validatable to validate Models
-  Window manager for creating Dialog


## Usage
Add the following Bootstrapper class to your startup project
### Bootstapper.cs

```cs

// Implement the BootstrapperBase class in your Startup Application
public class Bootstrapper : BootstrapperBase
{
    // User any IoC Container of Choice
    private YourContainer container;

    // Specify the list of Assemblies to search for Views or ViewModels
    // Follow MVVM Naming Convention as possible
    protected override IEnumerable<Assembly> SelectAssemblies()
    {     return new[] {
                typeof ( YourViewModel ).Assembly,
                typeof ( YourView ).Assembly
    };
 }

    //  Specify special Mappings for ViewModels that doesn't follow Naming Convention
    protected override Dictionary<Type, Type> SpecialViewModelMappings()
    {     return new Dictionary<Type, Type>() {
                {typeof ( YourCustomViewModel ), typeof ( TargetView )},
                {typeof ( YourCustomViewModel2 ), typeof ( TargetView  )},
            };
    }

    // Initialize the IoC Container for Run time 
    protected override void ConfigureForRuntime()
    {   container = new YourDIContainer();
        //Register Dependencies 
        container.Register(

                     //Dependency Registrations for Run time
                    // Repositories, Services etc
            );
        RegisterViewModels();
    }

    // Initialize the IoC Container for Design time 
    protected override void ConfigureForDesignTime()
    {   container = new YourContainer();
        //Register Dependencies 
        container.Register(
                    //Your Dependency Registrations for Design Time
                    // Mocked Data Implementations with In memory Data
            );
        RegisterViewModels();
    }


    // Registers all the Public ViewModels
    private void RegisterViewModels()
    {   // search the assembly and get types
        container.Register(Classes.FromAssembly(typeof( YourViewModel ).Assembly)
            .Where(x => x.Name.EndsWith("ViewModel"))
            .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
    }

    // Resolves the Requested Type from the container
    protected override object GetInstance(Type service, string key)
    {
        return string.IsNullOrWhiteSpace(key)
            ? container.Resolve(service)
            : container.Resolve(key, service);
    }
}

```
Call the Start( ) Method in your Entry point

```cs
// Interaction logic for App.xaml
    public partial class App : Application {
        private void App_OnStartup(object sender, StartupEventArgs e) {
            var bootstrapper = new Bootstrapper();
            bootstrapper.Start();
        }
    }
```
## View First Approach
To Automatically Set ViewModels as data context to respective views, use the following in  your .XAML file 
```cs
GenricMVVM:AutoViewModelLocator.AutoAttachViewModel="True" >
```
## ViewModel First Approach

ViewModel first apporach we use a ShellView window  as a base to render other usercontrols
####  ShellView .XAML
```XMl

   <!--Enable Auto Attaching Of ViewModel for just ShellViewModel-->
    mvvmFramework:AutoViewModelLocator.AutoAttachViewModel="True"
   
   <!--Child Views[should be usercontrols not windows] Will decide the Size of the Window-->
    SizeToContent="WidthAndHeight" WindowStartupLocation="CenterScreen"  >
    
    <!--DataTemplate Mapping of ViewModel to View-->
    <Window.Resources>
        <DataTemplate DataType="{x:Type viewModels:MainViewModel}">
            <views:MainView/>
        </DataTemplate>
    </Window.Resources>

    <Grid>
        <!--CurrentViewModel from ShellViewModel-->
        <ContentControl Content="{Binding CurrentViewModel}"/>
    </Grid>
```
####  ShellView Model.cs
```cs
Public Class ShellViewModel
{  
        // ShellViewModel is the BASE that displays other Views in it
        public class ShellViewModel : IHandle<SwitchToVm>, INotifyPropertyChanged {
        public ShellViewModel(IEventAggregator aggregator) {
            // Subscribed to Messages sent by other Views
            aggregator.Subscribe(this);
            // Initially set to MainViewModel
            CurrentViewModel = IoC.Get<MainViewModel>();
        }
        
        private object currentViewModel;
        public object CurrentViewModel {
            get { return currentViewModel; }
            private set {
                currentViewModel = value;
                OnPropertyChanged();
            }
        }

        // The Handle Function is called when ever Publishers send message
        // Changes the current view with the View passed in message
        public void Handle(SwitchToVm message) {
            CurrentViewModel = IoC.Get(message.ViewModel);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //Invoked to mention UI that property is changed
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
}
```
```cs
// Event Message Structure
public class SwitchToVm 
{
        public Type ViewModel { get; private set; }
        public SwitchToVm(Type viewModel) 
        {
            ViewModel = viewModel;
        }
}
```
#### To Change  the current view
publish a message with the target viewmodel to render
```cs
aggregator.Publish(new SwitchToVm(typeof (TargetViewModel)));
```
## EventAggregator

 In Message Sender class
```cs
 private  IEventAggregator aggregator;
  
 aggregator.Publish(new CustomMessage());
```
 In Subscriber Class
```cs
 public Class Subscriber : IHandle<CustomMessage>
 {
     private  IEventAggregator aggregator;
      subscriber(){aggregator.Subscribe(this);}
     public void Handle(CustomMessage message){
        // your Code
     }
  }
```

## RelayCommand

 use RelayCommand to delegate the action for ICommand
```cs
 public ICommand SubmitChangesCommand { get; }
 public MainViewModel(  ){
    SubmitChangesCommand = new RelayCommand(SubmitChanges, canSubmit);
 } 

 private void SubmitChanges(){    
 }

 private bool canSubmit(){ 
 }
```
## Validatable

 implement Validatable on any model with DataAnnotations. under the hood this implements INotifyDataErrorInfo sending error informatin to UI
```cs
 public class Employee : Validatable {
        private int id;
        private string name;
        private string emailAddress;

        [Required]
        public int Id {
            get { return id; }
            set {
                id = value; 
                OnPropertyChanged();
               }
        }

        [Required]
        public string Name {
            get { return name; }
            set {
                name = value; 
                OnPropertyChanged();
            }
        }

        [EmailAddress]
        public string EmailAddress {
            get { return emailAddress; }
            set {
                emailAddress = value; 
                OnPropertyChanged();
            }
        }
    }
```
set ValidatesOnNotifyDataErrors as True in View.XAML
```XML
 <TextBox x:Name="Name" Text="{Binding Student.Name, ValidatesOnNotifyDataErrors=True}"/>
```
## Design Time Data Rendering

It becomes tiresome to restart the application everytime to preview every shall chnages made. using design time data rendering, we can mock data and functionalities directly to the designer view.

Add the DesignTimeViewModelLocator.cs to your project
```cs
      // Resolves the View model of the View during the Design View
       public class DesignTimeViewModelLocator : IValueConverter {
        //Define static instance to access in the XMAL code
        public static DesignTimeViewModelLocator Instance = new DesignTimeViewModelLocator();

        static DesignTimeViewModelLocator() {
            if (Execution.InDesignMode) {
                //Execute the Bootstrapper and Initiate the IoC in Design Time
                var bootstrapper = new Bootstrapper();
                bootstrapper.Start();
            }
            // Do noting in Runtime
        }

        //implemented from IValueConverter Interface
        // Converts ViewModel type to object
        /// <param name="value">ViewModel Instance</param>
        /// <returns> returns the object of the type </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var assembliesForSearchingIn = AssemblySource.Instance;

            // Find the type
            var allExportedTypes = new List<Type>();
            foreach (var assembly in assembliesForSearchingIn) {
                allExportedTypes.AddRange(assembly.GetExportedTypes());
            }

            var viewModelType = allExportedTypes.First(t => t.FullName == value.ToString());
            // instantiate and Type object
            return IoC.GetInstance(viewModelType, null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return DependencyProperty.UnsetValue;
        }
    }
```
Add Design Time DataContext in View.XAML
```XML
d:DataContext="{Binding Source={d:DesignInstance viewModels: YourViewModel},
                Converter={x:Static views:DesignTimeViewModelLocator.Instance}}">
```
## License
[MIT](https://choosealicense.com/licenses/mit/)
