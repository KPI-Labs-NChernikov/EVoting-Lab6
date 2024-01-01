using System.Text;
using Algorithms.BlumBlumShub;

var bbsKeyGen = new BlumBlumShubKeysGenerator();
var bbs = new BBSXorEncryptionProvider();
var keys = bbsKeyGen.Generate();
var x0 = bbsKeyGen.GenerateSeed(keys.PublicKey.N);
var publicKey = keys.PublicKey.WithX0(x0);
var toEncrypt = Encoding.UTF8.GetBytes("Hello world");
var encrypted = bbs.Encrypt(toEncrypt, publicKey);
var decrypted = bbs.Decrypt(encrypted, keys.PrivateKey.WithX0(x0));
Console.WriteLine(Encoding.UTF8.GetString(decrypted));
