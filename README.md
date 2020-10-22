## Updating bosdyn.client

### Uninstalling the previous client
```
pip uninstall -y bosdyn.client
```
### Installing the new client
Navigate in your terminal to: 
```
spot-sdk/python/bosdyn-client
```
Run the following command to create the module:
```
export BOSDYN_SDK_VERSION=2.0.2
python setup.py bdist_wheel
```
next issue the following command:
```
pip install ./dist/bosdyn_client-2.0.2-py2.py3-none-any.whl
```

single snippet:
```
pip uninstall -y bosdyn.client
export BOSDYN_SDK_VERSION=2.0.2
python setup.py bdist_wheel
pip install ./dist/bosdyn_client-2.0.2-py2.py3-none-any.whl
```
### Installing .NET Core on Linux
If you are using some Linux distro, follow the instructions provided by MSFT:
https://docs.microsoft.com/en-us/dotnet/core/install/linux

### Building the project on Linux
Once you have installed .NET Core issue the following commands:
```
$ cd spot-server
$ dotnet restore
$ dotnet build
```

## Certificate configuration
req.conf:
```
[req]
distinguished_name = req_distinguished_name
req_extensions = v3_req
prompt = no
[req_distinguished_name]
C = BE
ST = West-Flanders
L = Oudenburg
O = Monstrey Technologies
OU = R&D
CN = *.spot.robot
[v3_req]
keyUsage = keyEncipherment, dataEncipherment
extendedKeyUsage = serverAuth
subjectAltName = @alt_names
[alt_names]
DNS.1 = id.spot.robot
```

ssl generation command
```
openssl genrsa -passout pass:1111 -des3 -out ca.key 4096
openssl req -passin pass:1111 -new -x509 -days 365 -key ca.key -out ca.crt -subj  "//CN=rootCA"
openssl genrsa -passout pass:1111 -des3 -out server.key 4096
openssl req -passin pass:1111 -new -key server.key -out server.csr -nodes -config req.conf
openssl x509 -req -passin pass:1111 -days 365 -in server.csr -CA ca.crt -CAkey ca.key -set_serial 01 -out server.crt
openssl rsa -passin pass:1111 -in server.key -out server.key
```
