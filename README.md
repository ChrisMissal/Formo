# Formo [![endorse](http://api.coderwall.com/chrismissal/endorsecount.png)](http://coderwall.com/chrismissal)

Formo was built on some code that I've used many times in the past. This time, however, I leveraged some fun dynamic goodness.

[![NuGet Status](http://nugetstatus.com/Formo.png)](http://nugetstatus.com/packages/Formo)

## How to use it

Given you have a few of the following settings in your app.config file, you can new up a `Configuration` object and call those settings directly through a property.

### The settings

```xml
<appSettings>
    <add key="RetryAttempts" value="5" />
    <add key="ApplicationBuildDate" value="11/4/1999 6:23 AM" />
</appSettings>
```

### The code

```csharp
dynamic config = new Configuration();
var retryAttempts1 = config.RetryAttempts;                 // returns 5 as a string
var retryAttempts2 = config.RetryAttempts(10);             // returns 5 if found in config, else 10
var retryAttempts3 = config.RetryAttempts(userInput, 10);  // returns 5 if it exists in config, else userInput if not null, else 10
```

Both of the values `userInput` and `10` will be ignored if the value has already been set in your file.

The Configuration class also has the ability to call dynamic methods with type arguments. (I know, right?!) This lets you call your property and cast it to the type of your choice.

```csharp
dynamic config = new Configuration();
var appBuildDate = config.ApplicationBuildDate<DateTime>();
```

### Specifying Culture

If you have dates in your settings file that need to be bound to a specific culture, you can do this on creation of the Configuration class.

```csharp
dynamic config = new Configuration(new CultureInfo("de"));
```

### Property Binding

You can also use Formo to bind settings values to properties on an object:

given:

```xml
<appSettings>
    <add key="SessionTimeout" value="20" />
    <add key="WebsiteSettingsSiteTitle" value="Cat Facts" />
</appSettings>
```

and...

```csharp
public class WebsiteSettings
{
    public int SessionTimeout { get; set; }
    public string SiteTitle { get; set; }
}
```

then...

```csharp
dynamic config = new Configuration();
var settings = config.Bind<WebsiteSettings>();
```

resulting in...

```csharp
settings.SessionTimeout = 20;
settings.SiteTitle = "Cat Facts";
```

### Configuration Section

You can use Formo on Configuration Sections

```xml
<configuration>
    <configSections>
        <section name="customSection" type="System.Configuration.NameValueSectionHandler"/>
    </configSections>
    <customSection>
        <add key="ApplicationBuildDate" value="11/4/1999 6:23 AM" />
    </customSection>
    <appSettings>
    </appSettings>
</configuration>
```

This still works from the previous example:

```csharp
dynamic config = new Configuration("customSection");
var appBuildDate = config.ApplicationBuildDate<DateTime>();
```

Remark the name of the section to load on the Configuration creation.
So far the only suported sections are based on `System.Configuration.NameValueSectionHandler`.

The Property Binding feature also works on sections.

## Installation

To install Formo, please use NuGet:

    Install-Package Formo

## Enhancements / Feedback

Use the issues link to get in touch with me about any improvements that could be made, or any bugs you encounter.

