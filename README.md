# secure-file

A small command-line utility for encrypting/decrypting arbitrary files using Rijndael method.

Encrypt:

    secure-file -encrypt <filename.ext> -secret <keyphrase> -out <filename.ext.enc>

Decrypt:

    secure-file -decrypt <filename.ext.enc> -secret <keyphrase> -out <filename.ext>
