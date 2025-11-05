// TrabajoTarjeta.Tests/ColectivoTest.cs
using NUnit.Framework;
using TrabajoTarjeta;
using System;


namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class ColectivoTest
    {
        [SetUp]
        public void Setup()
        {
            Tiempo.Now = () => new DateTime(2025, 4, 5, 10, 0, 0);
        }

        [Test]
        public void Colectivo_Constructor_ValoresCorrectos()
        {
            var colectivo = new Colectivo("Linea 123", false);
            Assert.AreEqual("Linea 123", colectivo.ObtenerLinea());
        }

        [Test]
        public void PagarCon_TarjetaConSaldo_Urbano_BoletoCreado()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 2000;
            var colectivo = new Colectivo("Linea 123", false);

            bool resultado = colectivo.PagarCon(tarjeta, colectivo, out Boleto boleto);

            Assert.IsTrue(resultado);
            Assert.IsNotNull(boleto);
            Assert.AreEqual("Linea 123", boleto.Linea);
            Assert.AreEqual(2000 - Colectivo.TARIFA_BASICA, tarjeta.Saldo);
        }

        [Test]
        public void PagarCon_TarjetaConSaldo_Interurbano_BoletoCreado()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 4000;
            var colectivo = new Colectivo("Linea 500", true);

            bool resultado = colectivo.PagarCon(tarjeta, colectivo, out Boleto boleto);

            Assert.IsTrue(resultado);
            Assert.IsNotNull(boleto);
            Assert.AreEqual("Linea 500", boleto.Linea);
            Assert.AreEqual(4000 - Colectivo.TARIFA_BASICA_INTERURBANO, tarjeta.Saldo);
        }

        [Test]
        public void PagarCon_TarjetaSinSaldo_Urbano_BoletoNoCreado()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 100; // ¡100 + 1200 = 1300 < 1580 → NO puede pagar!
            var colectivo = new Colectivo("Linea 123", false);

            bool resultado = colectivo.PagarCon(tarjeta, colectivo, out Boleto boleto);

            Assert.IsFalse(resultado);
            Assert.IsNull(boleto);
            Assert.AreEqual(100, tarjeta.Saldo);
        }

        [Test]
        public void ConstantesTarifas_ValoresCorrectos()
        {
            Assert.AreEqual(1580, Colectivo.TARIFA_BASICA);
            Assert.AreEqual(3000, Colectivo.TARIFA_BASICA_INTERURBANO);
        }
    }
}