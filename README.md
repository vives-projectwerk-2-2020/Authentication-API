# Documentatie Authenticatie API

* Code info

The api is writting in c# with asp.net core
It uses controller which are being called with requests through an url.
To request a specific method inside the controller change the url to [server]/api/[controller]/[methodname]

* sending requests

To send request to the API send requests to the url: [server]/api/user
The `user` is the name of the controller wich is contacted.
In that controller are multiple functions
To call specific functions change the url to:
-   [server]/api/user/login (post)
-   [server]/api/user/register (post)
-   [server]/api/user/delete (delete)

* login + authentication

When a user performs a correct login request with the good credentials, a JWT token will be created and send back to the frontend.
This token must be used when the user is trying to reach a function which need a authorized user to request it.

The token is created inside the login method, and will be set for 10 seconds.
To change the lifetime of the token, change the expire time.
`` Expires = DateTime.UtcNow.AddMinutes(10) ``

* further instructions & tips

The api send a request to another api linked with the database.
To make the project better and cleaner, use the code the make the token and checks in the other API
This does delete the extra linking with makes it faster and better.