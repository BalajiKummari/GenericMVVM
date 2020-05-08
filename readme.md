init
# GenericMVVM

A Micro MVVM Framework for WPF .NET Framework

## Installation

Use the  [nuget package manager](https://pip.pypa.io/en/stable/) to install GenericMVVM.

```bash
Install-Package Technovert.WPF.GenericMVVM 
```
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
### Bootstapper

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



## License
[MIT](https://choosealicense.com/licenses/mit/)
