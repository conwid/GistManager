# GistManager extension

This is a Visual Studio extension that let's you intuitively and comfortably manage your Github gists. 

You can download the extension from the Visual Studio MarketPlace:

You can check out my blog on the process of creating the extension:

## Contributing

If you have any ideas or requests, feel free to create an issue or submit a PR.

The file ClientInfo.cs is intentionally missing from the project, because it contains sensitive information (I know there are better ways to handle this, but for now it suits my purposes; that can be one of the PRs :) ). If you want to build your code, you first have to register your own Github app, then add the ClientInfo.cs with the following structure (and don't forget NOT to commit it :) ):

```csharp
namespace GistManager
{
    public static class ClientInfo
    {
        public const string ClientId = "your app's client id";
        public const string ClientSecret = "your app's client secret";
    }
}
```

## Features

List and search your private and public gists:
![](https://dotnetfalconcontent.blob.core.windows.net/gistextension/list.png)

Use drag-and-drop to create gists or add gistfiles to an already existing gist:
![](https://dotnetfalconcontent.blob.core.windows.net/gistextension/drop.gif)

Use drag-and-drop to drop the contents of a gistfile to the Visual Studio editor:
![](https://dotnetfalconcontent.blob.core.windows.net/gistextension/drag.gif)

Rename gist files:                       
![](https://dotnetfalconcontent.blob.core.windows.net/gistextension/rename.gif)

Manage your gists: copy URL to clipboard, delete or checkout an earlier version of the gist:
![](https://dotnetfalconcontent.blob.core.windows.net/gistextension/gistoptions.png)

Manage gistfiles: copy the raw URL of the file, delete the file or checkout an earlier version of the file:
![](https://dotnetfalconcontent.blob.core.windows.net/gistextension/gistfileoptions.png)
