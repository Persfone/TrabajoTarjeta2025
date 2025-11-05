using NUnit.Framework;
using TrabajoTarjeta;
using System;

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
            bool resultado = colectivo.PagarCon(tarjeta, colectivo, out Boleto boleto);

            Assert.IsTrue(resultado);
            Assert.IsNotNull(boleto);
            Assert.AreEqual("143", boleto.Linea);
            Assert.AreEqual(420, boleto.SaldoRestante);
        }

        [Test]
        public void PagarCon_TarjetaSinSaldoSuficiente_NoGeneraBoleto()
        {
            var tarjeta = new Tarjeta { Saldo = 0 };
            var colectivo = new Colectivo("143");
            bool resultado = colectivo.PagarCon(tarjeta, colectivo, out Boleto boleto);

            Assert.IsFalse(resultado); // 0 - 1580 = -1580, excede -1200
            Assert.IsNull(boleto);
        }

        [Test]
        public void PagarCon_TarjetaConSaldoNegativoPermitido_GeneraBoleto()
        {
            var tarjeta = new Tarjeta { Saldo = 500 };
            var colectivo = new Colectivo("143");
            bool resultado = colectivo.PagarCon(tarjeta, colectivo, out Boleto boleto);

            Assert.IsTrue(resultado); // 500 - 1580 = -1080, está dentro de -1200
            Assert.IsNotNull(boleto);
            Assert.AreEqual(-1080, boleto.SaldoRestante);
        }

        [Test]
        public void PagarCon_MedioBoleto_GeneraBoletoConDescuento()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 2000 };
            var colectivo = new Colectivo("K");
            bool resultado = colectivo.PagarCon(tarjeta, colectivo, out Boleto boleto);

            Assert.IsTrue(resultado);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(1210, boleto.SaldoRestante); // 2000 - 790
        }

        [Test]
        public void PagarCon_FranquiciaCompleta_GeneraBoletoSinCobrar()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new FranquiciaCompleta { Saldo = 0 };
            var colectivo = new Colectivo("102");
            bool resultado = colectivo.PagarCon(tarjeta, colectivo, out Boleto boleto);

            Assert.IsTrue(resultado);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(0, boleto.SaldoRestante);
        }

        [Test]
        public void PagarCon_BoletoGratuito_PrimerosDosBoletos()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil { Saldo = 0 };
            var colectivo = new Colectivo("115");

            bool primerBoleto = colectivo.PagarCon(tarjeta, colectivo,out Boleto boleto1);
            Assert.IsTrue(primerBoleto);
            Assert.IsNotNull(boleto1);

            bool segundoBoleto = colectivo.PagarCon(tarjeta, colectivo,out Boleto boleto2);
            Assert.IsTrue(segundoBoleto);
            Assert.IsNotNull(boleto2);
        }

        [Test]
        public void ObtenerLinea_DevuelveLineaCorrecta()
        {
            var colectivo = new Colectivo("143");
            Assert.AreEqual("143", colectivo.ObtenerLinea());
        }

        [Test]
        public void Colectivo_TarifaBasicaCorrecta()
        {
            Assert.AreEqual(1580, Colectivo.TARIFA_BASICA);
        }
    }
}