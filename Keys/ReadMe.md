## How to generate public / private key, JWK.

### openssl
[https://www.openssl.org/](https://www.openssl.org/)

1. Generate private key in PEM format.
>openssl genrsa -aes128 -out fd.key 2048

Protect your private key with password.

Here, the key is protected with AES-128. 
You can also use AES-192 or AES-256 (switches -aes192 and -aes256, respectively), but it’s best to stay away from the other algorithms (DES, 3DES, and SEED).

Key Generated in PEM format, with `-----BEGIN RSA PRIVATE KEY-----` header.

2. Generate corresponding public key in PEM format.
>openssl rsa -in fd.key -pubout -out fd-public.key

Use same password as in step 1 above.

3. Convert PEM to DER.

C# and Microsoft support RSA keys in DER binary format.
If you don't want to bring additional dependency convert PEM
format into binary DER format
>openssl rsa -inform PEM -in fd.key -outform DER -out fd.der

Use same password from step 1.

**Keep your private key away from public storage, like source control!**

4. Generate JWK.

Follow instructions on [Finastra Docs](https://developer.fusionfabric.cloud/documentation/oauth2-grants#jwk-auth) to generate JWK and create application registration

###### TL;DR
> Generate JWK here:
https://russelldavies.github.io/jwk-creator/.
>Remember value in field `KEY ID`
>
>Upload result to Finastra application.
>
>JWK Key should match configuration option`"JwkKeyId": "<PUBLIC_KEY_ID_REGISTERED_AT_FINASTRA>"`
