using NUnit.Framework;
using System;
using System.IO;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class BoletoTests
    {
        [Test]
        public void Constructor_CopiaDatosCorrectamenteDesdeTarjeta()
        {
            // Arrange
            var tarjeta = new Tarjeta
            {
                Saldo = 3450.50,
                TipoTarjeta = "Medio Boleto Estudiantil",
                Id = "SUBE-TEST-001"
            };
            string linea = "143";

            // Act
            var boleto = new Boleto(linea, tarjeta);

            // Assert
            Assert.AreEqual(linea, boleto.Linea);
            Assert.AreEqual(tarjeta.Saldo, boleto.SaldoRestante);
            Assert.AreEqual(tarjeta.TipoTarjeta, boleto.TipoTarjeta);
            Assert.AreEqual(tarjeta.Id, boleto.IdTarjeta);
            Assert.That(boleto.FechaEmision, Is.InRange(DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5)));
        }

        [Test]
        public void Imprimir_MuestraInformacionCompletaEnConsola()
        {
            // Arrange
            var tarjeta = new Tarjeta
            {
                Saldo = 10000,
                TipoTarjeta = "Franquicia Completa",
                Id = "SUBE-TEST-002"
            };
            var boleto = new Boleto("35", tarjeta);

            // Redirigimos salida de consola
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            boleto.Imprimir();
            string output = sw.ToString();

            // Assert: verificamos que toda la información esté impresa
            StringAssert.Contains("Boleto emitido para la línea: 35", output);
            StringAssert.Contains("Saldo restante en la tarjeta: 10000.00", output);
            StringAssert.Contains("Tipo de tarjeta: Franquicia Completa", output);
            StringAssert.Contains("ID de la tarjeta: SUBE-TEST-002", output);
            StringAssert.Contains("Fecha de emisión:", output);
        }

        [Test]
        public void Propiedades_SePuedenModificarCorrectamente()
        {
            // Arrange
            var tarjeta = new Tarjeta
            {
                Saldo = 2000,
                TipoTarjeta = "Sin Franquicia",
                Id = "SUBE-123"
            };

            var boleto = new Boleto("20", tarjeta);

            // Act
            boleto.TipoTarjeta = "Medio Boleto Estudiantil";
            boleto.TotalAbonado = 1580.5f;
            boleto.IdTarjeta = "SUBE-456";

            // Assert
            Assert.AreEqual("Medio Boleto Estudiantil", boleto.TipoTarjeta);
            Assert.AreEqual(1580.5f, boleto.TotalAbonado);
            Assert.AreEqual("SUBE-456", boleto.IdTarjeta);
        }
    }
}
