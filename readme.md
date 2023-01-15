Current Result:

```
RESULT name="Name" sa_construction_time=13161 ms 
sa_construction_memory=1089,02 lcp_naive_construction_time=14892 
lcp_kasai_construction_time=2374 
lcp_phi_construction_time=1698
```

After implementing IBaseArray interface:

```
RESULT name="Name" sa_construction_time=17994 ms 
sa_construction_memory=879,31 
lcp_naive_construction_time=14998 
lcp_kasai_construction_time=2480 
lcp_phi_construction_time=1720
```

English 100MB Text:
libsais: 14s
this implementation: 36,6s
my c implementation: 42s


# Compiling the Project
## Installing the SDK
The solution uses the .NET 6.0 framework. To compile and run it on Ubuntu 20.04 LTS the framework has to be installed first. This can be done in two steps as explained in the [.NET documentation](https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu#2004).:

1. Add the Microsoft package signing key to your list of trusted keys and add the package repository. To do that run the following commands.

    ```
    wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb

    sudo dpkg -i packages-microsoft-prod.deb

    rm packages-microsoft-prod.deb
    ```

2. To install the SDK run the following commands. The runtime is include in the SDK and does not need to be installed seperately.
    ```
    sudo apt-get update && \
     sudo apt-get install -y dotnet-sdk-6.0
    ```

For troubleshooting, other ways of installation (e.g. Snap or scripted install) as well as other plattforms check the [installation overview](https://learn.microsoft.com/en-us/dotnet/core/install/linux) of the .NET documentation.

## Compile and Run
For compiling simply run the following command from the .NET SDK:

```
dotnet build -o bin
```

The option ```-o``` is used to specify the output directory, in this case "bin".

