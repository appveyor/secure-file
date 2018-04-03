# secure-file

A small command-line utility for encrypting/decrypting arbitrary files using Rijndael method.

High-level scenario of using this utility in [AppVeyor CI](http://www.appveyor.com) environment:

- Encrypt file on development machine.
- Commit encrypted file to source control.
- Place "secret" to project environment variable.
- Decrypt file during the build.

System requirements:

* Windows or Linux
* .NET Core Runtime 2.0

## Encrypting file on development machine

Download `secure-file` utility by running the following PowerShell command on Windows machine:

    iex ((New-Object Net.WebClient).DownloadString('https://raw.githubusercontent.com/appveyor/secure-file/master/install.ps1'))

To install on Linux:

    curl -sflL 'https://raw.githubusercontent.com/appveyor/secure-file/master/install.sh' | bash -e -

To encrypt a file on Windows:

    appveyor-tools\secure-file -encrypt C:\path-to\filename-to-encrypt.ext -secret MYSECRET1234

To encrypt on Linux:

    ./appveyor-tools/secure-file -encrypt /path-to/filename-to-encrypt.ext -secret MYSECRET1234

Encrypted file will be saved in the same directory as the input file, but with the `.enc` extension added. You can optionally specify output file name with the `-out` parameter.

After that commit the encrypted file to source control.


## Decrypting files during an AppVeyor build

Put the "secret" value to the project environment variables on the _Environment_ tab of the project settings or in the `appveyor.yml` as a [secure variable](https://ci.appveyor.com/tools/encrypt):

```
environment:
  my_secret:
    secure: BSNfEghh/l4KAC3jAcwAjgTibl6UHcZ08ppSFBieQ8E=
```

To decrypt the file, add these lines to the `install` section of your project config:

```
install:
  - ps: iex ((New-Object Net.WebClient).DownloadString('https://raw.githubusercontent.com/appveyor/secure-file/master/install.ps1'))
  - cmd: appveyor-tools\secure-file -decrypt path-to\encrypted-filename.ext.enc -secret %my_secret%
  - sh: ./appveyor-tools/secure-file -decrypt path-to/encrypted-filename.ext.enc -secret $my_secret
```

The line starting with `cmd:` will run on Windows-based images only and the line starting with `sh:` on Linux.

> Note that file won't be decrypted on Pull Request builds as secure variables are not set during PR build.
