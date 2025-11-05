using NUnit.Framework;
using System;
using System.IO;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class BoletoTests
    {
        [Test]
        public void Constructor_AsignaPropiedadesCorrectamente()
        {
            // Arrange
            var tarjeta = new Tarjeta
            {
                Saldo = 1500,
                TipoTarjeta = "Comun",
                Id = "ABC123"
            };

            string linea = "143";

            // Act
            var boleto = new Boleto(linea, tarjeta);

            // Assert
            Assert.AreEqual(linea, boleto.Linea);
            Assert.AreEqual(tarjeta.Saldo, boleto.SaldoRestante);
            Assert.AreEqual(tarjeta.TipoTarjeta, boleto.TipoTarjeta);
            Assert.AreEqual(tarjeta.Id, boleto.IdTarjeta);

            // Fecha de emisión: aseguramos que sea reciente
            Assert.That(boleto.FechaEmision, Is.InRange(DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5)));
        }

        [Test]
        public void Imprimir_MuestraInformacionCorrecta()
        {
            // Arrange
            var tarjeta = new Tarjeta
            {
                Saldo = 1000,
                TipoTarjeta = "Estudiantil",
                Id = "TAR123"
            };
            var boleto = new Boleto("102", tarjeta);

            // Redirigimos la salida de consola para capturarla
            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            boleto.Imprimir();
            var output = sw.ToString();

            // Assert: verificamos que todas las líneas aparezcan
            StringAssert.Contains("Boleto emitido para la línea: 102", output);
            StringAssert.Contains("Saldo restante en la tarjeta: 1000.00", output);
            StringAssert.Contains("Tipo de tarjeta: Estudiantil", output);
            StringAssert.Contains("ID de la tarjeta: TAR123", output);
        }

        [Test]
        public void Propiedades_SettersYGetters_FuncionanCorrectamente()
        {
            var boleto = new Boleto("K", new Tarjeta { Saldo = 500, TipoTarjeta = "Comun", Id = "ID001" })
            {
                TotalAbonado = 1580f,
                TipoTarjeta = "Medio",
                IdTarjeta = "ID002"
            };

            Assert.AreEqual(1580f, boleto.TotalAbonado);
            Assert.AreEqual("Medio", boleto.TipoTarjeta);
            Assert.AreEqual("ID002", boleto.IdTarjeta);
        }
    }
}
