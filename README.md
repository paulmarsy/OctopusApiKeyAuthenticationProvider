# OctopusApiKeyAuthenticationProvider
Octopus Deploy Authentication using an API key
https://my.octopus.server.com/api/users/authenticate/ApiKeyAuth/{apiKey}

#### Examples
https://my.octopus.server.com/api/users/authenticate/ApiKeyAuth/API-Z8UHYOM0REERAPNGWVXPGSI5KW8

Or without 'API-' prefix
https://my.octopus.server.com/api/users/authenticate/ApiKeyAuth/Z8UHYOM0REERAPNGWVXPGSI5KW8

A variation of this functionality would be the creation and authentication of a single use link / nonce

#### Authentication REST Method
![New API Method](https://cloud.githubusercontent.com/assets/1769405/22402083/4fe86142-e5e2-11e6-9263-074c5211a675.png)
#### Configuration Settings Page
![Configuration Page](https://cloud.githubusercontent.com/assets/1769405/22402089/ce3ef380-e5e2-11e6-80a1-0606d473bb11.png)
#### '.\Octopus.Server.exe configure' Options
![Configure Options](https://cloud.githubusercontent.com/assets/1769405/22402119/933ff8e6-e5e3-11e6-8147-50d54367d3b9.png)
#### ExtensionConfiguration table entry
![dbo.ExtensionConfiguration](https://cloud.githubusercontent.com/assets/1769405/22402132/05e29192-e5e4-11e6-9286-7846573522d1.png)
