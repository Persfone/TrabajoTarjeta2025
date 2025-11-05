// TrabajoTarjeta.Tests/BoletoTest.cs
using System;
using System.IO;
using NUnit.Framework;
using TrabajoTarjeta;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class BoletoTest
    {
        [SetUp]
        public void Setup()
        {
            Tiempo.Now = () => new DateTime(2025, 4, 5, 10, 0, 0);
        }

        [Test]
        public void Boleto_Constructor_ValoresCorrectos()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 1500;
            tarjeta.TipoTarjeta = "Test";
            tarjeta.Id = "TEST-123";

            var boleto = new Boleto("Linea 123", tarjeta);

            Assert.AreEqual("Linea 123", boleto.Linea);
            Assert.AreEqual(1500, boleto.SaldoRestante);
            Assert.AreEqual("Test", boleto.TipoTarjeta);
            Assert.AreEqual("TEST-123", boleto.IdTarjeta);
            Assert.That((Tiempo.Now() - boleto.FechaEmision).TotalSeconds, Is.LessThan(1));
        }

        [Test]
        public void Boleto_Constructor_ConTarjetaEspecial_ValoresCorrectos()
        {
            var tarjeta = new MedioBoletoEstudiantil();
            tarjeta.Saldo = 2000;

            var boleto = new Boleto("Linea 456", tarjeta);

            Assert.AreEqual("Linea 456", boleto.Linea);
            Assert.AreEqual(2000, boleto.SaldoRestante);
            Assert.AreEqual("Medio Boleto Estudiantil", boleto.TipoTarjeta);
            Assert.IsNotNull(boleto.IdTarjeta);
        }

        [Test]
        public void Boleto_Imprimir_MuestraInformacionCorrecta()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 1000;
            tarjeta.TipoTarjeta = "Test";
            tarjeta.Id = "TEST-123";

            var boleto = new Boleto("Linea 999", tarjeta);
            var output = new StringWriter();
            Console.SetOut(output);

            boleto.Imprimir();

            string salida = output.ToString();
            StringAssert.Contains("Linea 999", salida);
            StringAssert.Contains("1000.00", salida);
            StringAssert.Contains("Test", salida);
            StringAssert.Contains("TEST-123", salida);
        }

        [Test]
        public void Boleto_Propiedades_SoloLectura()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 1000;
            var boleto = new Boleto("Linea 123", tarjeta);

            boleto.TipoTarjeta = "Nuevo Tipo";
            boleto.IdTarjeta = "Nuevo ID";
            boleto.TotalAbonado = 100;

            Assert.AreEqual("Nuevo Tipo", boleto.TipoTarjeta);
            Assert.AreEqual("Nuevo ID", boleto.IdTarjeta);
            Assert.AreEqual(100, boleto.TotalAbonado);
        }
    }
}