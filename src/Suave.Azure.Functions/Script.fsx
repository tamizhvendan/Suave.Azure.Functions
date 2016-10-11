#I "./../../packages"
#r "Suave/lib/net40/Suave.dll"
#r "System.Net.Http/lib/net46/System.Net.Http.dll"
open System

let uri = new Uri("http://tamazurefun.azurewebsites.com/api/Hello/Suave?foo=bar&bar=baz")

uri.Host
uri.Port
uri.AbsolutePath
uri.AbsoluteUri
uri.LocalPath