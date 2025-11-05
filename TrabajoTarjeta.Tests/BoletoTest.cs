using NUnit.Framework;
using TrabajoTarjeta;
using System;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class BoletoTests
    {
        [Test]
        public void CrearBoleto_GuardaLineaYSaldo()
        {
            var tarjeta = new Tarjeta { Saldo = 2500 };
            var boleto = new Boleto("143", tarjeta);

            Assert.AreEqual("143", boleto.Linea);
            Assert.AreEqual(2500, boleto.SaldoRestante);
        }

        [Test]
        public void CrearBoleto_GuardaTipoTarjeta()
        {
            var tarjeta = new Tarjeta { Saldo = 2000 };
            var boleto = new Boleto("K", tarjeta);

            Assert.AreEqual("Sin Franquicia", boleto.TipoTarjeta);
        }

        [Test]
        public void CrearBoleto_GuardaIdTarjeta()
        {
            var tarjeta = new Tarjeta { Saldo = 2000 };
            var boleto = new Boleto("143", tarjeta);

            Assert.IsNotNull(boleto.IdTarjeta);
            Assert.IsTrue(boleto.IdTarjeta.StartsWith("SUBE-"));
        }

        [Test]
        public void CrearBoleto_TieneFechaEmision()
        {
            var tarjeta = new Tarjeta { Saldo = 2000 };
            var boleto = new Boleto("102", tarjeta);

            Assert.IsNotNull(boleto.FechaEmision);
            Assert.LessOrEqual((DateTime.Now - boleto.FechaEmision).TotalSeconds, 1);
        }

        [Test]
        public void CrearBoleto_MedioBoleto_GuardaTipoCorrecto()
        {
            var tarjeta = new MedioBoletoEstudiantil { Saldo = 2000 };
            var boleto = new Boleto("143", tarjeta);

            Assert.AreEqual("Medio Boleto Estudiantil", boleto.TipoTarjeta);
        }

        [Test]
        public void CrearBoleto_FranquiciaCompleta_GuardaTipoCorrecto()
        {
            var tarjeta = new FranquiciaCompleta { Saldo = 0 };
            var boleto = new Boleto("K", tarjeta);

            Assert.AreEqual("Franquicia Completa", boleto.TipoTarjeta);
        }

        [Test]
        public void CrearBoleto_BoletoGratuito_GuardaTipoCorrecto()
        {
            var tarjeta = new BoletoGratuitoEstudiantil { Saldo = 0 };
            var boleto = new Boleto("115", tarjeta);

            Assert.AreEqual("Boleto Gratuito Estudiantil", boleto.TipoTarjeta);
        }

        [Test]
        public void CrearBoleto_ConSaldoNegativo_GuardaSaldoCorrectamente()
        {
            var tarjeta = new Tarjeta { Saldo = -500 };
            var boleto = new Boleto("143", tarjeta);

            Assert.AreEqual(-500, boleto.SaldoRestante);
        }

        [Test]
        public void CrearBoleto_DespuesDePago_SaldoActualizado()
        {
            var tarjeta = new Tarjeta { Saldo = 2000 };
            var colectivo = new Colectivo("K", false);
            tarjeta.Pagar(1580, colectivo);
            var boleto = new Boleto("143", tarjeta);

            Assert.AreEqual(420, boleto.SaldoRestante);
        }

        [Test]
        public void CrearBoleto_ConDiferentesLineas()
        {
            var tarjeta1 = new Tarjeta { Saldo = 2000 };
            var tarjeta2 = new Tarjeta { Saldo = 2000 };

            var boleto1 = new Boleto("143", tarjeta1);
            var boleto2 = new Boleto("K", tarjeta2);

            Assert.AreEqual("143", boleto1.Linea);
            Assert.AreEqual("K", boleto2.Linea);
        }
    }
}