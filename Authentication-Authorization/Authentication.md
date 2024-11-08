# Authentication and Authorization

## Authentication:
Authentication is the process of varyfying the identity of user or systems.

**Types of Authentication:**
1. `Basic Authentication:` Client sends the username and password encoded in Base64 in the HTTP header with every request. The server decodes and validates the credentials.
2. `Token-Based Authentication (JWT, OAuth2):` User logs in and receives a token (e.g., JWT). The token is sent with every request as a Bearer token in the Authorization header.
3. `Session-Based Authentication:`
   1. The server creates a session after the user logs in.
   2. A session ID is stored on the server and shared with the client via a cookie.
   3. For subsequent requests, the client sends the session cookie.
4. `Multi-Factor Authentication (MFA):`
   1. Requires two or more verification steps, such as:
      1. Password (something you know)
      2. OTP via SMS (something you have)
      3. Biometric data (something you are)
5. `Social Authentication (OAuth, OpenID Connect):`
   1. Users log in via third-party providers (Google, Facebook, GitHub).
   2. The app uses OAuth to authenticate the user by redirecting them to the provider.
6. `Biometric Authentication:`
   1. Uses fingerprint, facial recognition, or voice authentication.
   2. Biometric authentication relies on hardware and software integration.
7. `API Key Authentication:`
   1. An API key is generated for the client.
   2. The key is sent in headers or query parameters for each request.
8. `Device-Based Authentication`: Device-Based Authentication verifies a user's identity based on the device they are using to access a service.
   1. Banks often remember trusted devices.
9. `Risk-Based Authentication (Adaptive Authentication)`: It is an advanced security method that dynamically adjusts the authentication requirements based on the risk level associated with each login attempt.
   1.  RBA evaluates various factors (like `location, device, IP address, time, and behavior patterns`) to determine if additional verification is needed.
10. `Single Sign-On (SSO)`: Single Sign-On (SSO) is an authentication method that enables users to access multiple applications or services with `one set of login credentials (e.g., a single username and password)`.
    1.  Accessing multiple Google services (e.g., Gmail, Google Drive, YouTube) after logging in once to a Google account.

---
### JWT
A JWT (JSON Web Token) is a `compact, self-contained token` used for securely transmitting information `between parties`. It consists of three parts. Each part is `Base64Url-encoded` and concatenated using `dots (.)`.
1. `Header (algorithm and token type):` The header contains metadata about the token, including the `algorithm` used to generate the signature and the `token type`.
   1. `alg:` Algorithm used for signing (e.g., `HS256, RS256`).
   2. `typ:` Token type (usually "JWT")
2. `Payload (claims/data)`: The payload contains the claims, which are statements about an entity (like the user) and additional metadata.
   1. `Registered claims:` Standardized fields (e.g., iss, exp, sub).
   2. `Public claims:` Custom claims defined by your application (e.g., role, userId).
   3. `Private claims:` Shared information between issuer and consumer.
3. `Signature (verification):` The signature is used to verify that the message wasn’t tampered with.
---
#### **Trade-off:**
**Pros of JWT**
1. **Stateless Authentication:** No need to store session data on the server.
2. **Scalability:** Works well with distributed systems or microservices.
3. **Compact & Portable:** Can be easily transmitted in HTTP headers.
4. **Self-contained:** Carries all the necessary information within the token.

**Cons of JWT**
1. **No Server-Side Revocation:** Once a JWT is issued, it can’t be revoked unless there’s a `blacklist mechanism`.
2. **Payload Size:** Large payloads increase the size of each request.
3. **Security Risk:** If the token is compromised, the attacker has access until it expires.
   
---
#### How JWT works?
     +--------+                               +---------------+
     |        |--(A)- Authorization Request ->|   Resource    |
     |        |                               |     Owner     |
     |        |<-(B)--- Access Token ---------|    (User)     |
     | Client |                               +---------------+
     | (SPA)  |
     |        |                               +---------------+
     |        |--(C)----- Access Token ------->| Authorization |
     |        |                               |     Server    |
     |        |<-(D)--- Protected Resource ---|   (Identity   |
     +--------+                               |     Server)   |
     |        |                               +---------------+
     |        |
     |        |                               +---------------+
     |        |--(E)----- Access Token ------>|    Resource   |
     |        |                               |     Server    |
     |        |<-(F)--- Protected Resource ---|    (API)      |
     +--------+                               +---------------+


1. `Login / Authentication Request:` The user sends their credentials (e.g., email and password) to the authentication server via a login endpoint.
2. `Verification and Token Generation:`
   1. The `server validates the credentials` (by checking against a database).
   2. If valid, it generates a `JWT token` containing user data (e.g., user_id, roles) and signs it with a `secret or private key`.
3. `Token Sent to Client:`The JWT is returned to the user in the response, usually in a `JSON response.`
4. `Client Stores the Token:` The client stores the JWT in a local storage, session storage, or cookie.
5. `Accessing Protected Resources:` For every request to a protected resource, the client sends the JWT token in the Authorization header.
6. `Token Validation on Server:` The server extracts and verifies the JWT from the request header.
   1. If the token is valid (signature matches, and it hasn’t expired), the request is allowed to proceed.
   2. If invalid or expired, the server returns a 401 Unauthorized error.
7. `Response:` If authenticated, the server sends the requested data to the client

