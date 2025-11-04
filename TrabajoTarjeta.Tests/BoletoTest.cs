using NUnit.Framework;
using TrabajoTarjeta;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class BoletoTests
    {
        [Test]
        public void CrearBoleto_GuardaLineaYSaldo()
        {
            var boleto = new Boleto("143", 2500);

            Assert.AreEqual("143", boleto.Linea);
            Assert.AreEqual(2500, boleto.SaldoRestante);
        }
    }
}
