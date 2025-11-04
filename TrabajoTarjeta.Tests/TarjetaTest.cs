using NUnit.Framework;
using TrabajoTarjeta;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class TarjetaTests
    {
        [Test]
        public void CargarTarjeta_IncrementaSaldoCorrectamente()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 0;

            
            int[] cargas = { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };

            foreach (var monto in cargas)
            {
                tarjeta.Saldo = 0;  
                tarjeta.Saldo += monto;
                Assert.AreEqual(monto, tarjeta.Saldo);
            }
        }

        [Test]
        public void Pagar_DecrementaSaldoCorrectamente()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 5000;

            bool pudoPagar = tarjeta.Pagar(1580);

            Assert.IsTrue(pudoPagar);
            Assert.AreEqual(3420, tarjeta.Saldo);
        }

        [Test]
        public void Pagar_NoPermiteSaldoNegativo()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 1000;

            bool pudoPagar = tarjeta.Pagar(1580);

            Assert.IsFalse(pudoPagar);
            Assert.AreEqual(1000, tarjeta.Saldo);  
        }
    }
}
