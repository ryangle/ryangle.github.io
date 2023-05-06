using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Parameters;

namespace BouncyCastleCase;

public class CertificateUtility
{
    public static void GenerateCertificate(string filename,
        string password,
        string signatureAlgorithm,
        X509Name issuer,
        X509Name subject,
        DateTime notBefore,
        DateTime notAfter,
        string friendlyName,
        int keyStrength = 2048)
    {
        SecureRandom random = new SecureRandom(new CryptoApiRandomGenerator());

        KeyGenerationParameters keyGenerationParameters = new(random, keyStrength);
        RsaKeyPairGenerator keyPairGenerator = new();
        keyPairGenerator.Init(keyGenerationParameters);
        var subjectKeyPair = keyPairGenerator.GenerateKeyPair();

        ISignatureFactory signatureFactory = new Asn1SignatureFactory(signatureAlgorithm, subjectKeyPair.Private, random);

        X509V3CertificateGenerator certificateGenerator = new();

        var spki = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(subjectKeyPair.Public);

        //设置一些扩展字段
        //允许作为一个CA证书（可以颁发下级证书或进行签名）
        certificateGenerator.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(true));
        //使用者密钥标识符
        certificateGenerator.AddExtension(X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifier(spki));
        //授权密钥标识符
        certificateGenerator.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifier(spki));

        certificateGenerator.AddExtension(X509Extensions.ExtendedKeyUsage.Id, true, new ExtendedKeyUsage(KeyPurposeID.id_kp_serverAuth));

        var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(long.MaxValue), random);
        certificateGenerator.SetSerialNumber(serialNumber);

        certificateGenerator.SetIssuerDN(issuer);
        certificateGenerator.SetSubjectDN(subject);
        certificateGenerator.SetNotBefore(notBefore);
        certificateGenerator.SetNotAfter(notAfter);
        certificateGenerator.SetPublicKey(subjectKeyPair.Public);

        var certificate = certificateGenerator.Generate(signatureFactory);

        //生成cer证书
        //var certificate2 = new X509Certificate2(DotNetUtilities.ToX509Certificate(certificate))
        //{
        //    FriendlyName = friendlyName,
        //};
        ////cer公钥文件
        //var bytes = certificate2.Export(X509ContentType.Cert);
        //using (var fs = File.Create(filename))
        //{
        //    fs.Write(bytes, 0, bytes.Length);
        //}

        //另一种代码生成p12证书的方式
        //certificate2 =
        //             certificate2.CopyWithPrivateKey(DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters)keyPair.Private));

        //var bytes2 = certificate2.Export(X509ContentType.Pfx, password);
        //using (var fs = File.Create(filename))
        //{
        //    fs.Write(bytes2, 0, bytes2.Length);
        //}

        //生成p12证书
        var certEntry = new X509CertificateEntry(certificate);
        var store = new Pkcs12StoreBuilder().Build();
        store.SetCertificateEntry(friendlyName, certEntry);
        var chain = new X509CertificateEntry[1];
        chain[0] = certEntry;
        store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(subjectKeyPair.Private), chain);
        using (var fs = File.Create(filename))
        {
            store.Save(fs, password.ToCharArray(), random);
        };

    }
    public static void Test()
    {
        //颁发者DN
        var issuer = new X509Name(new List<DerObjectIdentifier>
        {
            X509Name.C,
            X509Name.O,
            X509Name.OU,
            X509Name.L,
            X509Name.ST
        }, new Dictionary<DerObjectIdentifier, string>
        {
            [X509Name.C] = "CN",
            [X509Name.O] = "BouncyCastleCase",
            [X509Name.OU] = "BouncyCastleCase RSA CA",
            [X509Name.L] = "Fengtai",
            [X509Name.ST] = "Beijing",
        });
        //使用者DN
        var subject = new X509Name(new List<DerObjectIdentifier>
        {
            X509Name.C,
            X509Name.O,
            X509Name.CN
        }, new Dictionary<DerObjectIdentifier, string>
        {
            [X509Name.C] = "CN",
            [X509Name.O] = "ICH",
            [X509Name.CN] = "*.bouncyxastlexase.com"
        });

        var password = "123456";    //证书密码
        var signatureAlgorithm = "SHA256WITHRSA"; //签名算法

        //生成证书
        GenerateCertificate("BouncyCastleCase.pfx", password, signatureAlgorithm, issuer, subject, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddYears(2), "BouncyCastleCase passport");

        //加载证书
        X509Certificate2 pfx = new X509Certificate2("BouncyCastleCase.pfx", password, X509KeyStorageFlags.Exportable);

        var keyPair = DotNetUtilities.GetKeyPair(pfx.GetRSAPrivateKey());

        var subjectPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public);
        var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);

        var privateKey = Base64.ToBase64String(privateKeyInfo.ParsePrivateKey().GetEncoded());
        var publicKey = Base64.ToBase64String(subjectPublicKeyInfo.GetEncoded());

        Console.WriteLine("私钥：");
        Console.WriteLine(privateKey);

        Console.WriteLine("公钥：");
        Console.WriteLine(publicKey);

        var data = "hello world";

        Console.WriteLine($"加密原文：{data}");
    }
}
