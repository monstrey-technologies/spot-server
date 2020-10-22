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

Then you have to copy the generated files to the location where the program needs them:
```
$ cp server.crt spot-server/SpotServer/bin/Debug/netcoreapp3.1/
$ cp server.key spot-server/SpotServer/bin/Debug/netcoreapp3.1/
$ cp ca.crt     spot-server/SpotServer/bin/Debug/netcoreapp3.1/
```

## Start the server
The server runs in port 443, so you have to execute it as root/sudo:
```
$ sudo spot-server/SpotServer/bin/Debug/netcoreapp3.1/SpotServer
[sudo] password for root: 
D1022 17:09:16.631023 Grpc.Core.Internal.UnmanagedLibrary Attempting to load native library "spot-server/SpotServer/bin/Debug/netcoreapp3.1/runtimes/linux/native/libgrpc_csharp_ext.x64.so"
D1022 17:09:16.782109 Grpc.Core.Internal.NativeExtension gRPC native library loaded successfully.
Virtual spot 001 is active on port 443
Press any key to shutdown
```
