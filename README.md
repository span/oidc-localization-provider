# oidc-localization-provider
Sets localization based on ui_cultures parameter in OIDC

## Usage
This package sets up a custom `QueryStringRequestCultureProvider` that processes the query parameter `ui_locales` specified in OpenIDConnect.

It can be configured like this:

```
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services)
{
    services
        .AddLocalization()
        .AddOidcLocalization();
    services.AddControllers();
}
```
