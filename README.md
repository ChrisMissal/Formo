Formo [![endorse](http://api.coderwall.com/chrismissal/endorsecount.png)](http://coderwall.com/chrismissal)
=====

Formo was built on some code that I've used many times in the past. This time, however, I leveraged some fun dynamic goodness.

[![NuGet Status](http://nugetstatus.com/Formo.png)](http://nugetstatus.com/packages/Formo)

## How to use it

Given you have a few of the following settings in your app.config file, you can new up a `Configuration` object and call those settings directly through a property.

***The settings***

    <appSettings>
        <add key="RetryAttempts" value="5" />
    </appSettings>

***The code***

    var config = new Configuration();
    var retryAttempts = config.RetryAttempts;

Alternately, you can call `RetryAttempts` as a method. You'll want to call this as a method if you're not certain it exists (or has a value) in your settings.

    var retryAttempts = config.RetryAttempts(10);

If you're pulling your default value from another source, and it has the chance of providing a null value, you can keep providing arguments as params.

    var retryAttempts = config.RetryAttempts(userInput, 10);

Both of the values `userInput` and `10` will be ignored if the value has already been set in your file.

### More!

The Configuration class also has the ability to call dynamic methods with type arguments. (I know, right?!) This lets you call your property and cast it to the type of your choice.

    var configuration = new Configuration();
    var appBuildDate = configuration.ApplicationBuildDate<DateTime>();

## Enhancements / Feedback / Issues

Use the issues tab to get in touch with me about any improvements that could be made, or any bugs you encounter.

