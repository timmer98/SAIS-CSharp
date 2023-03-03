# Suffix Array and LCP Consruction Algorithm
## Suffix Array Induced Sorting
The suffix array induced sorting algorithm is a linear time suffix array construction algorithm by Yuta Mori. This project is for learning purposes only and not at all optimized for performance. For production use the highly optimized [libsais](https://github.com/IlyaGrebnov/libsais) library by Ilya Grebnov should be used. Benchmarks showed that libsais is around 6 times faster and uses less memory.

## Longest Common Prefix
This project also contains implementations of two non trivial LCP construction algorithms - one using the inverse suffix array (KasaiLinearTimeLcpStrategy - [Kasai Algorithm](http://dx.doi.org/10.1007/3-540-48194-X_17)) and another (slighty faster) building a permuted lcp array (PhiLinearTimeLcpArray). 

# Compiling the Project
## 1. Installing the SDK
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

## 2. Compile
For compiling simply run the following command from the .NET SDK:

```
dotnet build -o bin
```

The option ```-o``` is used to specify the output directory, in this case "bin".

## 3. Run
To run the project navigate to the output folder (e.g. "bin") and run the file ```TextIndexierung.Console```. For help with the parameters run ```TextIndexierung.Console --help```.
