# 说明
本扩展是在webapi中使用RSA加密的扩展

![TFS Build](https://cqgis.visualstudio.com/_apis/public/build/definitions/8b214f0e-c703-40ab-b972-7a347c6b7c9c/19/badge)

[cqgis.extensions.aspnetcore.ssl](https://www.nuget.org/packages/cqgis.extensions.aspnetcore.ssl)

[cqgis.extensions.ssl](https://www.nuget.org/packages/cqgis.extensions.ssl)

## 生成公/私钥

采用openssl 命令生成私钥和公钥

私钥：
+ 命令：` openssl genrsa -out rsa_1024_priv.pem 1024 `
+ 生成的内容：
    ```
    -----BEGIN RSA PRIVATE KEY-----
    MIICXAIBAAKBgQDJQrluPaAoXlzM19vT/EPAyM7p6wD3rNumGbBFKA10GvffVwrX
    B0H++BXRbEjPQAK1dzWo+50LoDu1ctXZWDeDP24gp+s7Kg1ZKU21mXSJkXNd3PvA
    gi6KUicHorJiVqSseFlZ6XbhD42bmxQ5vqjlLrVTTomCgdYEB/rVYyJXywIDAQAB
    AoGBAIJNp/6++XWJeq3JR/NczYUAvi5mz46LpP5KfXyY7PTyDO/VzBfiBBGPD5ky
    cYV1XQueLejFVy1SxbW5rsj0O6OfnYUsl1SvLfRtNen6mcQwKbmZVbxNLiIDIden
    K5SJXk6be+2l5UaLxezutwidniGKkrucjeSPgtPs9MgqIKwhAkEA/CzHiGx/Uakb
    gBdS358jWLDIhn3AJ7Yj/PgPRDt7rfB0ZzGfx79O9r6xTRkRMyEVWyVduT0M2w3G
    xoMXpxVKsQJBAMxQPVnYqKf+kdS17dI0J2jBdmw44sQ4RTbXv2oSN/n5S3D3mTPw
    LDSxAoS9YAkfKbBEB1+tbrkJY/kOuvvicTsCQGLIB8Fg/t62J5edi1EdvXLYjTRh
    vgvQRXiqkxBfXKteLbrerYBJaF0vhbR1xU8J/pudn4EYcRi95MeOQo2owYECQEPr
    7OlggHkQxTQ8ouo9klhd8Ft78c8fkcrwoF7H3boJaU4N0e2VS2zc56RvPGzr4jUJ
    +JWTF3sGJykV/41bMx8CQBZjCwRPea4ZDn3voDIF4EBWQoA5/wuSArzQoLyPATTq
    U59ILZRWpeY7EQE59fAs3S0Ae+MAO8Emx+4rNDbqb5A=
    -----END RSA PRIVATE KEY-----

    ```

公钥：
+ 命令：`openssl rsa -pubout -in rsa_1024_priv.pem -out rsa_1024_pub.pem `
+ 生成的内容：
    ```
    -----BEGIN PUBLIC KEY-----
    MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDJQrluPaAoXlzM19vT/EPAyM7p
    6wD3rNumGbBFKA10GvffVwrXB0H++BXRbEjPQAK1dzWo+50LoDu1ctXZWDeDP24g
    p+s7Kg1ZKU21mXSJkXNd3PvAgi6KUicHorJiVqSseFlZ6XbhD42bmxQ5vqjlLrVT
    TomCgdYEB/rVYyJXywIDAQAB
    -----END PUBLIC KEY-----

    ```


## 加密

本程序中涉及到两种类别的加密：非对称加密、对称加密。

+ 非对称加密：非对称加密使用公钥和私钥。
    公钥加密信息，私钥解密信息；
    私钥签名信息，公钥验证签名。
+ 对称加密：
    通过设定盐值，对信息进行加密、解密处理

## 信息的传递

信息的传递都通过上面提到的对称加密和非对称加密来处理；服务端保留私钥，客户端获取公钥。

#### token

客户端再向服务端发起的http请求中，应当在header中加入token信息：
1. token的主键根据服务端的约定，两边保持一值；
2. token值中，同时加入用于验证的信息和对称加密的盐值，用点号(.)隔开，然后使用公钥进行加密：
> 如用于验证的值为123,盐值为abc，对称加密的函数为fx(a,b),公钥加密的函数为qx(a),则最后token的值为:
> qx(fx(123,abc)+"."+abc)

#### post 加密

客户端向服务端发起的POST请求应该采用加密传输，加密的方法是：用盐值（和token中设置的一致）对信息进行加密，然后用公钥对信息加密。
> 如用于验证的值为123,盐值为abc，对称加密的函数为fx(a,b),公钥加密的函数为qx(a),则在body中的值应该为:
> qx(fx(123,abc))

#### 返回的信息

1. 服务端返回的信息，无论是get还是post，都会进行加密和签名处理，加密和签名的方法为：对信息用对称盐值进行加密，然后用公钥对加密后信息的md5验证。加密和信息和验证的信息之间用点号（.）隔开:
  >如信息为123,盐值为abc,对称加密的函数为fx(a,b),私钥签名信息为zx(a),最后返回的信息为:
  > y=fx(123,abc)
  > return y+"."+zx(md5(y))

2. 客户端收到消息后，从结果中分离信息和验证信息，首先用公钥对签名进行验证，如果验证不通过，则说明数据被篡改。
3. 验证通过后，使用约定的加密盐值对信息进行解密处理。


## SSLMiddleware

在ASP.NET CORE中使用此中间件，可以自动处理加密和解密的信息
#### 使用方法

1. ConfigureServices方法中，使用
```
  public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            #region rsa
            services.AddRSAManager(t =>{
                      t.DecryptKey = "..";
                      t.PrivateKey_Des ="...";
                      t.PublicKey_Des ="...";
                },t =>{
                      t.HearderTokenKey = "token"; //如果使用token验证，这里应该和客户端保持一值
               });
            #endregion 
            ...
        }
```

2. Configure方法中，使用
```
  public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseReturnSSL();
            ...
            app.UseMvc();

        }
```
  **此管道必须放在最前面**



#### 说明

+ 对于http请求信息的自动解密，目前只处理了post请求；
+ 如果post请求没有经过加密处理，也可以继续后续的操作；





