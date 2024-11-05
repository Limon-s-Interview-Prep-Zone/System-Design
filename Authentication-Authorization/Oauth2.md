# [Oauth2:](https://datatracker.ietf.org/doc/html/rfc6749)
OAuth 2.0, which stands for `“Open Authorization”`, is a standard designed to allow a website or application to access resources hosted by other web apps on behalf of a user.
- OAuth 2.0 is an `authorization protocol` and NOT an `authentication protocol`.

## Why do we need oauth2?
Many websites allow users to log in using their `Google or Facebook accounts`. This uses the OAuth2 framework, where the user grants the site permission to access specific information from their social profile (like name, email) without giving away their `Google/Facebook password`. Using `oauth2` the `website (client)` redirects the user to `Google/Facebook (authorization server)`, which asks the user to confirm access. If approved, an access token is issued to the website, allowing limited access to the user’s profile data. However, without `oauth2` the website would need the `user’s Google or Facebook credentials`, creating `security risks`, as this approach could expose the user's password and make the website liable for handling it securely.

## Oauth2 role
In Sarah's scenario with Trello and Google Calendar:

1. Resource Owner: Sarah, who owns the calendar data.
2. Client: Trello, the application requesting access to Sarah’s calendar.
3. Authorization Server: Google’s OAuth server, which authenticates Sarah and grants Trello access.
4. Resource Server: Google Calendar API, which hosts the calendar data and accepts access tokens for authorized requests.


Workflow:<br>

          +--------+                               +---------------+
          |        |--(A)- Authorization Request ->|   Resource    |
          |        |                               |     Owner     |
          |        |<-(B)-- Authorization Grant ---|    (Sarah)    |
          |        |                               +---------------+
          |        |
          |        |                               +---------------+
          |        |--(C)-- Authorization Grant -->| Authorization |
          | Trello |                               |     Server    |
          | Client |                               |   (Google)    |
          |        |<-(D)----- Access Token -------|               |
          |        |                               +---------------+
          |        |
          |        |                               +---------------+
          |        |--(E)----- Access Token ------>|    Resource   |
          |        |                               |     Server    |
          |        |<-(F)--- Protected Resource ---|   (Google     |
          +--------+                               |   Calendar)   |
                                                  +---------------+



**Step-by-Step Explanation**
1. `(A) Authorization Request:` Trello sends Sarah (the Resource Owner) a request to connect with her Google Calendar. Sarah is redirected to Google’s authorization page.
2. `(B) Authorization Grant:` Sarah approves the request, granting Trello permission to access her calendar data. This approval sends an Authorization Grant back to Trello.
3. (C) `Authorization Grant to Authorization Server:` Trello sends the Authorization Grant to Google’s Authorization Server and requests an access token.
4. `(D) Access Token:` Google’s Authorization Server validates the grant and issues an Access Token to Trello.
5. `(E) Access Token to Resource Server:` Trello uses the Access Token to make an authorized request to the Google Calendar API (the Resource Server), requesting access to Sarah’s calendar events.
6. `(F) Protected Resource:` Google Calendar API (the Resource Server) verifies the Access Token and, if valid, serves the requested calendar data back to Trello.

## Authorization Grant
In OAuth2, the Authorization Grant is a credential representing the resource owner’s consent for the client to access resources on their behalf. OAuth2 defines four primary types of authorization grants, each suited for different scenarios.

### Authorization Grant Code
The authorization code is obtained by using an authorization server as an intermediary between the client and resource owner.  Instead of requesting authorization directly from the resource owner, the client directs the resource owner to an authorization server (IdentityServer), which in turn directs the resource owner back to the client with the authorization code.


          +--------+                                +---------------+
          |        |--(A)- Authorization Request -->|   Resource    |
          |        |                                |    Owner      |
          |        |<-(B)-- Authorization Code -----|               |
          | Client |                                +---------------+
          |        |
          |        |                                +---------------+
          |        |--(C)---- Authorization Code -->| Authorization |
          |        |                                |     Server-   |
          |        |<-(D)------- Access Token ------|IdentityServer |
          +--------+                                +---------------+


### Implicit Grant
In the implicit flow, instead of issuing the client an authorization code, the client is issued an access token directly (as the result of the resource owner authorization).  The grant type is implicit, as no intermediate credentials (such as an authorization code) are issued (and later used to obtain an access token). For example- Sarah logs into a weather app that needs access to her Google Calendar events. When she grants permission, Google directly provides the access token to the app (which runs entirely in the browser).

          +--------+                               +---------------+
          |        |--(A)- Authorization Request -->|   Resource    |
          |        |                               |    Owner      |
          | Client |<-(B)---- Access Token --------|               |
          |        |                               +---------------+
          +--------+


### Resource Owner Password Credentials Grant
The resource owner password credentials (i.e., username and password) can be used directly as an authorization grant to obtain an access token.  The credentials should only be used when there is a high degree of trust between the resource owner and the client (e.g., the client is part of the device operating system or a highly privileged application), and when other authorization grant types are not available (such as an authorization code).


### Client Credentials Grant
Server-to-server or backend services where there’s no end-user interaction, and the client is trusted to access resources directly.

          +--------+                               +---------------+
          |        |--(A)---- Client Credentials -->| Authorization |
          | Client |                                |     Server    |
          |        |<-(B)------ Access Token -------|               |
          +--------+                                +---------------+

### Summary
| Grant Type                | Primary Use Case                    | Security Level | Example Scenario                                        |
|---------------------------|-------------------------------------|----------------|---------------------------------------------------------|
| **Authorization Code**    | Server-side web apps               | High           | Trello accessing Google Calendar                        |
| **Implicit**              | Single-page apps (SPAs)            | Moderate       | A weather app accessing user location                   |
| **Resource Owner Password** | Highly trusted clients (legacy, internal) | Lower | Bank mobile app accessing user account data             |
| **Client Credentials**    | Backend server-to-server communication | High       | A logging service interacting with an email API         |


## Access Token and Refresh token
### Access Token
Access tokens are credentials used to access protected resources.  An access token is a string representing an authorization issued to the client.  The string is usually opaque to the client.  Tokens represent specific scopes and durations of access, granted by the resource owner, and enforced by the resource server and authorization server.

### Refresh Token
Refresh tokens are credentials used to obtain access tokens. Refresh tokens are issued to the client by the authorization server and are used to obtain a new access token when the current access token becomes invalid or expires. Unlike access tokens, refresh tokens are intended for use only with authorization servers and are never sent to resource servers.

        +--------+                                           +---------------+
        |        |--(A)------- Authorization Grant --------->|               |
        |        |                                           |               |
        |        |<-(B)----------- Access Token -------------|               |
        |        |               & Refresh Token             |               |
        |        |                                           |               |
        |        |                            +----------+   |               |
        |        |--(C)---- Access Token ---->|          |   |               |
        |        |                            |          |   |               |
        |        |<-(D)- Protected Resource --| Resource |   | Authorization |
        | Client |                            |  Server  |   |     Server    |
        |        |--(E)---- Access Token ---->|          |   |               |
        |        |                            |          |   |               |
        |        |<-(F)- Invalid Token Error -|          |   |               |
        |        |                            +----------+   |               |
        |        |                                           |               |
        |        |--(G)----------- Refresh Token ----------->|               |
        |        |                                           |               |
        |        |<-(H)----------- Access Token -------------|               |
        +--------+           & Optional Refresh Token        +---------------+

- (A)  The client requests an access token by authenticating with the authorization server and presenting an authorization grant.
- (B)  The authorization server authenticates the client and validates the authorization grant, and if valid, issues an access token and a refresh token.
- (C)  The client makes a protected resource request to the resource server by presenting the access token.
- (D)  The resource server validates the access token, and if valid, serves the request. 
- (E)  Steps (C) and (D) repeat until the access token expires.  If the client knows the access token expired, it skips to step (G); otherwise, it makes another protected resource request.
- (F)  Since the access token is invalid, the resource server returns an invalid token error.


### What is the best place for storing token?
1. Local Storage: For non-sensitive data that should persist between sessions, such as user settings or cached data for offline use.
2. Session Storage: For temporary data needed only within the current tab, such as form data or transient state.
3. Cookies: For session management, tracking, and storing user-specific data that the server needs to access with each request.
4. HTTP-Only Cookies: For securely storing authentication tokens or session identifiers in a way that prevents access from JavaScript, making it safer against XSS attacks.

| Feature                | Local Storage          | Session Storage       | Cookies               | HTTP-Only Cookies    |
|------------------------|------------------------|-----------------------|-----------------------|-----------------------|
| **Storage Limit**      | ~5-10 MB               | ~5 MB                | ~4 KB per cookie      | ~4 KB per cookie      |
| **Persistence**        | Until explicitly deleted | Until tab/window closes | Configurable (expires) | Configurable (expires) |
| **Accessible by JavaScript** | Yes             | Yes                  | Yes (unless HttpOnly) | No                    |
| **Scope**              | Per domain             | Per tab & domain      | Per domain & path     | Per domain & path     |
| **Use Case**           | Long-term storage      | Temporary session data | Auth, tracking       | Secure auth tokens    |
| **Vulnerability**      | XSS                    | XSS                   | XSS, CSRF             | CSRF (XSS-protected)  |

#### Recommendation:
1. Use HTTP-only cookies when security is a priority, as they prevent XSS-based token theft. They are especially suitable for applications with a backend server and support CSRF token-based protection(set `samesite=strict`).
2. Consider Local Storage only if your application is a pure SPA, and you’re implementing strict security practices to prevent XSS attacks. It’s suitable if you need the token to persist across sessions.
3. Use Session Storage for temporary tokens or data that should be cleared once the tab is closed, but be cautious of XSS vulnerabilities.