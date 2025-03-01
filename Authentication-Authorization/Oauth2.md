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
2. `(B) Authorization Grant:` Sarah approves the request, granting Trello permission to access her calendar data. This approval sends an `Authorization Grant` back to Trello.
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

---

## Clients:
In OAuth2, the "Client" is an application (such as a web, mobile, or desktop app) that requests access to resources on behalf of the "Resource Owner," usually the user.
1. **Confidential Client:** A Client that can keep its credentials (Client ID and Client Secret) secure. Examples include server-side applications (e.g., web apps running on a backend server).
2. **Public Client:** A Client that cannot securely store its credentials (no Client Secret), typically a client-side application such as a Single Page Application (SPA) or a mobile app.

## Protocol Endpoints:
   1. **Authorization Endpoint (`/authorize`):** Used to initiate the `OAuth2 flow and get user consent`.
        ```bash
        GET https://authorization-server.com/authorize? response_type=code& client_id=your-client-id& redirect_uri=https://your-app.com/callback& scope=openid%20profile& state=random-state-value
        ```
      * ***`Scope:`*** In OAuth 2.0, `scope` is a parameter that defines the level of access the Client (the app) is requesting from the Resource Server (such as Google, GitHub, or Facebook) on behalf of the user. It allows the Authorization Server to limit the client's access to specific resources or actions, ensuring that clients only receive the permissions necessary for their purposes. 
   2. **Token Endpoint (`/token`):** Used to exchange `authorization codes` for access tokens, and to request tokens in other flows (e.g., Client Credentials).
      
        ```bash
        POST https://authorization-server.com/token
        Content-Type: application/x-www-form-urlencoded
        grant_type=authorization_code&
        code=authorization-code&
        redirect_uri=https://your-app.com/callback&
        client_id=your-client-id&
        client_secret=your-client-secret
        ```
      * grant_type: The type of grant being used (e.g., authorization_code).
      * code: The Authorization Code received in the previous step.
      * redirect_uri: The same URI that was used in the authorization request.
      * client_id: The Client ID registered with the Authorization Server.
      * client_secret: The Client Secret (if applicable) to authenticate the Client.
   3. **Introspection Endpoint (`/introspect`):** Used to validate tokens.
        ```bash
        POST /revoke HTTP/1.1
        Host: auth.example.com
        Content-Type: application/x-www-form-urlencoded
        Authorization: Basic BASE64(CLIENT_ID:CLIENT_SECRET)

        token=ACCESS_TOKEN
        ```
   4. **Revocation Endpoint (`/revoke`):** Used to revoke tokens.
        ```bash
        POST /revoke HTTP/1.1
        Host: auth.example.com
        Content-Type: application/x-www-form-urlencoded
        Authorization: Basic BASE64(CLIENT_ID:CLIENT_SECRET)

        token=ACCESS_TOKEN
        ```
   5. **UserInfo Endpoint (`/userinfo`):** Used to retrieve user profile information using an access token. 
        ```bash
        GET https://resource-server.com/user/profile Authorization: Bearer access-token-value
        ```
   6. **Refresh Token Endpoint (`/token`):** Used to exchange a refresh token for a new access token.
        ```bash
        POST /token HTTP/1.1
        Host: auth.example.com
        Content-Type: application/x-www-form-urlencoded

        grant_type=refresh_token&refresh_token=REFRESH_TOKEN&client_id=CLIENT_ID&client_secret=CLIENT_SECRET
        ```
---
## OpenID Connect:
`OpenID Connect (OIDC)` is an authentication protocol built on top of `OAuth 2.0` that allows clients to verify the identity of a user. It provides a standardized way to authenticate users and obtain identity-related claims. While OAuth 2.0 is used for authorization (accessing resources), `OpenID Connect` adds authentication capabilities. OIDC enables applications to verify the identity of users and obtain basic profile information in a secure, standardized way.<br>
`OIDC allows clients`, typically `web or mobile apps`, to confirm a user’s identity based on the authentication performed by an Authorization Server and to obtain user information (such as their name, email, or profile picture) through an ID token.

***Key Points:***
1. **ID Token:** `A JWT (JSON Web Token)` issued by the Authorization Server, which confirms the user’s identity and contains claims about the user.
   - The ID Token includes essential claims like sub (subject identifier, or unique user ID), name, email, and exp (expiration time).
   - It’s digitally signed, allowing the client to verify the user’s identity and trust the contents.
2. **Authorization Endpoint:** Used by the client to initiate authentication.
The client redirects the user to this endpoint to authenticate and authorize access to their data.
1. **Token Endpoint:** After the user is authenticated, the client can use this endpoint to obtain tokens, including the access token (for API access) and the ID token (for authentication).
2. **UserInfo Endpoint:** A standard endpoint defined by OIDC where the client can retrieve additional user profile information.
   - Accessed with an access token, this endpoint provides details about the user that the client has permission to view.

### Why do we need OpenId Connect?
OpenID Connect is crucial when your app needs to verify the identity of a user and securely authenticate them. It’s a way to ensure that the user who logs into the app is the one they claim to be, often through third-party services (e.g., Google, Facebook).

### Scope
In OAuth 2.0, `scope` is a parameter that defines the level of access the Client (the app) is requesting from the Resource Server (such as Google, GitHub, or Facebook) on behalf of the user. It allows the Authorization Server to limit the client's access to specific resources or actions, ensuring that clients only receive the permissions necessary for their purposes.

**Types of Scopes:**
1. **Read-only:** Grants the client application permission to view the data but not modify it.
2. **Read-write:** Grants the client application permission to both read and modify the data.
3. **Admin access:** Grants full administrative control, including the ability to create, modify, and delete resources.
4. **User-specific scopes:** Access to specific user-related data like email, contacts, photos, or calendar.
5. **Custom scopes:** Some APIs might define custom scopes for their own needs, such as access to specific resources or actions (e.g., "premium_features" or "file_upload").

### API Resources
API Resources are the actual data or services that a client application wants to access. These are usually exposed by backend APIs, which are protected by OAuth 2.0. The client must have an access token to interact with these resources.

### Identity Resources
Identity Resources represent user-specific data (claims) that define who the user is. In OpenID Connect, identity resources are used to authenticate the user and return user-specific information, like the user's name, email, and profile picture. These resources are typically returned as part of the ID Token.

