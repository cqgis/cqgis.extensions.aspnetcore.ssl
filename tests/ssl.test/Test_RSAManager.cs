using cqgis.extensions.aspnetcore.ssl;
using Microsoft.Extensions.DependencyInjection;
using System;
using cqgis.extensions.ssl;
using Xunit;
using Xunit.Abstractions;

namespace ssl.test
{
    public class Test_RSAManager
    {
        private readonly ITestOutputHelper _output;
        private ServiceProvider sp;

        public Test_RSAManager(ITestOutputHelper output)
        {
            _output = output;
            IServiceCollection sc = new ServiceCollection()
                .AddRSAManager(t =>
                {
                    #region 
                    t.DecryptKey = "pwdlock";
                    t.PrivateKey_Des =
                        "W/JlZbE5pDeSfnEiTlJm+oMRSV6ANh+4cYFpaFEXsEckQH9SBRR3IzORQZZEylsdwDuBiOR+XTiM0HIIZtTcIgvJtUmeAz7UMnR1huthevzieC/YnrKGL+CxWNnh3M4r3++duivvVHBBC0Io7TreaKlUHiotvefcypdwl9uLhd9kgkBBgX8AwZKOGUz4GXQrAzqp9D69yefz+bsCIwuh/pW4/nNi4c3qwuyUtGuIqToOAPVnT03D3JYp8onFMkotfLuAU24ifv8/pwftSllJIS6uXK0h9P9hGrgXqWL1BLHTlDI/3cRLvTyue/D/fE0dSyONkKrAi7b0hCDqg0nQWYpLDpNqr96Q1LWEdS8PqYbNjEyTpM75ib1zrjuM6ol6JsE+iwojFjiHG1+/Yg1z43+n0vq/WTknUtTo+xdBtbV4/vc5Ul5XMkm/rL0v6D5/2PsfK7zekIuQ136lYr/fc+8awNm07J13khbP9IAxXBwKYC+zSER3WTlnQpghqTnOoUwmoNppu7+YvncVmaH/IUgNp6sHHWHXOz6XnjqSWaG2gcDtTeBZPjyHyGDYkKqoPS2KK9jPkZ29QJYmM9lg23X6PtS7vpSpfqsfLMtCWKsMBUCN2nO4+XmgO6+s/oYe4Raf5UCg+UMq1TMLHzywOebmuF2owd0HEU3nrzzV5/M+t4Fh4JAMakDWRS3RAmqLTCAvM/rzkRwYjzCZeVyITU2KwOR6hvQL7P0W4tvaHxUvzNzAk+qsEghg1Bzf/3oW4E5JghbxxWZ7MXuM5vSHOA6kqXwcvxgDkTZlF6WnvXwPPDiYuQuLFWIt6wfYVZJn4QXpgFLWGxQ9C8/x9OI3A5nG2JG807ofZSBcqYTJJmY85yhHeQ0qrKhjnAJUUN2bEhD/sjfQrhDEF2EDB2oNn6zwN2Noy6LSly9DkgcUbKjE81n7BIpTYOxXhZe9JlfylHDjjdurhsmyJYVXQiDwsky8c10nWYEjAdX4SJO1eM77GUXbUENazRCV3YgF0MKdDzY8SLwLQB4zVAJxvN6Yv6fBfP/iS75dOCgzaw/+MWA2GmxlnM6lbeNwpGSAAAzxsHcGG0zP9qPH9nKVJ8BJbOQQDKe28u2sfTw7FjpDm8N2DvJzwt3QTh9haxfFnozu54ZNhn90LKFrxpE1oZVJ9UJtB65ytdqljeT8yZ8VdcD8N8dTLnu8SpTfVe4u3iIvpDrv1DdFPSutbWw8Auk87w==";
                    t.PublicKey_Des =
                        "W/JlZbE5pDeSfnEiTlJm+oMRSV6ANh+4cYFpaFEXsEckQH9SBRR3IzORQZZEylsdwDuBiOR+XTiM0HIIZtTcIgvJtUmeAz7UMnR1huthevzieC/YnrKGL+CxWNnh3M4r3++duivvVHBBC0Io7TreaKlUHiotvefcypdwl9uLhd9kgkBBgX8AwZKOGUz4GXQrAzqp9D69yefz+bsCIwuh/pW4/nNi4c3qwuyUtGuIqToOAPVnT03D3JYp8onFMkotfLuAU24ifv8/pwftSllJIS6uXK0h9P9hGrgXqWL1BLHJLc/0IverMlJhcVsYze7alJ32jRrpS6cLpHjySAwFaw==";
                    #endregion
                });
            this.sp = sc.BuildServiceProvider();
        }


        [Fact]
        public void Test_keyOption_Null()
        {
            IServiceCollection sc = new ServiceCollection().AddRSAManager(t => { });
            var sp = sc.BuildServiceProvider();

            var option = sp.GetRequiredService<RSAKeyOption>();
            option.ShouldNotNull();

            Assert.Throws<ArgumentNullException>(() => sp.GetRequiredService<IEncryptManager>());
        }


        [Theory]
        [InlineData("zhya")]
        [InlineData("111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111")]
        public void Test_Encrypt(string value)
        {
            var em = this.sp.GetRequiredService<IEncryptManager>();

            var pwd = em.Encrypt(value);

            pwd.ShouldNotNullOrEmpty();
            pwd.ShouldBe(t => t != value);
            _output.WriteLine(pwd);

            Assert.Throws<ArgumentNullException>(() => em.Encrypt(string.Empty));

        }


        [Theory]
        [InlineData("n6iYUQXRfKZrdpXyTkXP9aa6J6MlzP1P9uxMXnEBxXuKdEJswLVIWPlVlhn6nCVytE2NKbPXj6jhmxLWCJ1NG/NQKXCg7Go47mJs7YGxk6LNM/rjGQ95xdUED59sPbdDhBTlOYLYzA0n2WDxkYaeALzEKOLHmeNd+2nBAu8Xy2E=","zhya")]
        [InlineData("uF09NEw7FkoNHmr6gDtBbPRrq9dfzE67ozVUTdaKf4CP4Bd2AEr699KXGORGLqJBlJW1ltNDcVIGshl5tg9bIJaLdRbo1vruVzOH9sfVYKD3uTJGMyC98DVjBR3y8uagcRb6SbwiPdOmRdd/PGQDUxZ0SLtxmJofkm+V8UAJyOCKULZeYdgsMK81lUGUbc+MRfjsmFgIfimuUN0NMYDR6x/ZYWWqOZPsSx+62wQaaWP/yqRDYkUuQNNqsuNO5ZCrCyHN/o/K90iPMA7CDpXQE5uLmKLCCAkCrqqYRlYFYpZO1NSlOd2a+/G9zj+5oC1Cw552caYLQqPZRSKr6yqRpQ==", "111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111")]
        public void Test_Descrypt(string descrypt,string pwd)
        {
            var em = this.sp.GetRequiredService<IEncryptManager>();
           
            var value = em.DecryptData(descrypt);
            value.ShouldBe(pwd);

            Assert.Throws<ArgumentNullException>(() => em.DecryptData(string.Empty));
        }



        [Theory]
        [InlineData("zhya")]
        [InlineData("111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111")]
        public void Test_Sign_Verify(string value)
        {
            var em = this.sp.GetRequiredService<IEncryptManager>();
            var data = value;
            var sign = em.SignInfo(data);

            sign.ShouldNotNullOrEmpty();
            sign.ShouldBe(t => t != data);

            var isvalid = em.Verify(data, sign);
            isvalid.ShouldBe(true);
        }

    }
}
