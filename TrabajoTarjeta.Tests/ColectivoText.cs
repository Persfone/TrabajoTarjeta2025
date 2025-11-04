using NUnit.Framework;
using TrabajoTarjeta;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class ColectivoTests
    {
        [Test]
        public void PagarCon_TarjetaConSaldoSuficiente_GeneraBoleto()
        {
            var tarjeta = new Tarjeta { Saldo = 2000 };
            var colectivo = new Colectivo("143");

            bool resultado = colectivo.PagarCon(tarjeta, out Boleto boleto);

            Assert.IsTrue(resultado);
            Assert.IsNotNull(boleto);
            Assert.AreEqual("143", boleto.Linea);
            Assert.AreEqual(420, boleto.SaldoRestante);
        }

        [Test]
        public void PagarCon_TarjetaSinSaldo_NoGeneraBoleto()
        {
            var tarjeta = new Tarjeta { Saldo = 1000 };
            var colectivo = new Colectivo("143");

            bool resultado = colectivo.PagarCon(tarjeta, out Boleto boleto);

            Assert.IsFalse(resultado);
            Assert.IsNull(boleto);
        }
    }
}
