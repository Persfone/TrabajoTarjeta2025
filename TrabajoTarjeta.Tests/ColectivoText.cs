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
            var colectivo = new Colectivo("143", false);
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
            var colectivo = new Colectivo("143", false);
            bool resultado = colectivo.PagarCon(tarjeta, colectivo, out Boleto boleto);
            Assert.IsFalse(resultado);
            Assert.IsNull(boleto);
        }

        [Test]
        public void PagarCon_TarjetaConSaldoNegativoPermitido_GeneraBoleto()
        {
            var tarjeta = new Tarjeta { Saldo = 500 };
            var colectivo = new Colectivo("Gálvez", true);
            bool resultado = colectivo.PagarCon(tarjeta, colectivo, out Boleto boleto);
            Assert.IsTrue(resultado);
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
            var colectivo = new Colectivo("K", false);
            bool resultado = colectivo.PagarCon(tarjeta, colectivo, out Boleto boleto);
            Assert.IsTrue(resultado);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(1210, boleto.SaldoRestante);
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
            var colectivo = new Colectivo("102", false);
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
            var colectivo = new Colectivo("115", false);
            bool primerBoleto = colectivo.PagarCon(tarjeta, colectivo, out Boleto boleto1);
            Assert.IsTrue(primerBoleto);
            Assert.IsNotNull(boleto1);

            System.Threading.Thread.Sleep(5100);

            bool segundoBoleto = colectivo.PagarCon(tarjeta, colectivo, out Boleto boleto2);
            Assert.IsTrue(segundoBoleto);
            Assert.IsNotNull(boleto2);
        }

        [Test]
        public void ObtenerLinea_DevuelveLineaCorrecta()
        {
            var colectivo = new Colectivo("143", false);
            Assert.AreEqual("143", colectivo.ObtenerLinea());
        }

        [Test]
        public void Colectivo_TarifaBasicaCorrecta()
        {
            Assert.AreEqual(1580, Colectivo.TARIFA_BASICA);
        }

        // ============================================
        // TESTS PARA COLECTIVOS INTERURBANOS
        // ============================================

        [Test]
        public void Colectivo_TarifaInterurbanaCorrecta()
        {
            Assert.AreEqual(3000, Colectivo.TARIFA_BASICA_INTERURBANO);
        }

        [Test]
        public void ColectivoInterurbano_CobraTarifaInterurbana()
        {
            var tarjeta = new Tarjeta { Saldo = 5000 };
            var colectivoInterurbano = new Colectivo("Galvez", true);

            bool resultado = colectivoInterurbano.PagarCon(tarjeta, colectivoInterurbano, out Boleto boleto);

            Assert.IsTrue(resultado);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(2000, boleto.SaldoRestante);
        }

        [Test]
        public void ColectivoUrbano_CobraTarifaNormal()
        {
            var tarjeta = new Tarjeta { Saldo = 5000 };
            var colectivoUrbano = new Colectivo("143", false);

            bool resultado = colectivoUrbano.PagarCon(tarjeta, colectivoUrbano, out Boleto boleto);

            Assert.IsTrue(resultado);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(3420, boleto.SaldoRestante);
        }

        [Test]
        public void ColectivoInterurbano_SinSaldoSuficiente_NoGeneraBoleto()
        {
            var tarjeta = new Tarjeta { Saldo = 2000 };
            var colectivoInterurbano = new Colectivo("Baigorria", true);

            bool resultado = colectivoInterurbano.PagarCon(tarjeta, colectivoInterurbano, out Boleto boleto);

            Assert.IsFalse(resultado);
            Assert.IsNull(boleto);
        }

        [Test]
        public void ColectivoInterurbano_PermiteSaldoNegativo()
        {
            var tarjeta = new Tarjeta { Saldo = 2000 };
            var colectivoInterurbano = new Colectivo("Galvez", true);

            bool resultado = colectivoInterurbano.PagarCon(tarjeta, colectivoInterurbano, out Boleto boleto);

            Assert.IsTrue(resultado);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(-1000, boleto.SaldoRestante);
        }

        [Test]
        public void ColectivoInterurbano_NoPermiteSaldoNegativoMayorA1200()
        {
            var tarjeta = new Tarjeta { Saldo = 1500 };
            var colectivoInterurbano = new Colectivo("Baigorria", true);

            bool resultado = colectivoInterurbano.PagarCon(tarjeta, colectivoInterurbano, out Boleto boleto);

            Assert.IsFalse(resultado);
            Assert.IsNull(boleto);
        }

        [Test]
        public void ColectivoInterurbano_MedioBoleto_AplicaDescuento()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 5000 };
            var colectivoInterurbano = new Colectivo("Galvez", true);

            bool resultado = colectivoInterurbano.PagarCon(tarjeta, colectivoInterurbano, out Boleto boleto);

            Assert.IsTrue(resultado);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(3500, boleto.SaldoRestante);
        }

        [Test]
        public void ColectivoInterurbano_FranquiciaCompleta_ViajeGratis()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new FranquiciaCompleta { Saldo = 1000 };
            var colectivoInterurbano = new Colectivo("Baigorria", true);

            bool resultado = colectivoInterurbano.PagarCon(tarjeta, colectivoInterurbano, out Boleto boleto);

            Assert.IsTrue(resultado);
            Assert.IsNotNull(boleto);
            Assert.AreEqual(1000, boleto.SaldoRestante);
        }

        [Test]
        public void ColectivoInterurbano_BoletoGratuito_PrimerosDosGratis()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil { Saldo = 1000 };
            var colectivoInterurbano = new Colectivo("Galvez", true);

            bool primerBoleto = colectivoInterurbano.PagarCon(tarjeta, colectivoInterurbano, out Boleto boleto1);
            Assert.IsTrue(primerBoleto);
            Assert.AreEqual(1000, boleto1.SaldoRestante);

            System.Threading.Thread.Sleep(5100);

            bool segundoBoleto = colectivoInterurbano.PagarCon(tarjeta, colectivoInterurbano, out Boleto boleto2);
            Assert.IsTrue(segundoBoleto);
            Assert.AreEqual(1000, boleto2.SaldoRestante);
        }

        [Test]
        public void ColectivoInterurbano_BoletoGratuito_TercerViajePagaTarifaCompleta()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil { Saldo = 5000 };
            var colectivoInterurbano = new Colectivo("Baigorria", true);

            colectivoInterurbano.PagarCon(tarjeta, colectivoInterurbano, out _);
            System.Threading.Thread.Sleep(5100);
            colectivoInterurbano.PagarCon(tarjeta, colectivoInterurbano, out _);
            System.Threading.Thread.Sleep(5100);

            bool tercerBoleto = colectivoInterurbano.PagarCon(tarjeta, colectivoInterurbano, out Boleto boleto3);
            Assert.IsTrue(tercerBoleto);
            Assert.AreEqual(2000, boleto3.SaldoRestante);
        }

        [Test]
        public void Trasbordo_EntreUrbanoeInterurbano_FuncionaCorrectamente()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new Tarjeta { Saldo = 10000 };
            var colectivoUrbano = new Colectivo("143", false);
            var colectivoInterurbano = new Colectivo("Galvez", true);

            colectivoUrbano.PagarCon(tarjeta, colectivoUrbano, out Boleto boleto1);
            double saldoDespuesPrimero = boleto1.SaldoRestante;
            Assert.AreEqual(8420, saldoDespuesPrimero);

            System.Threading.Thread.Sleep(1000);

            colectivoInterurbano.PagarCon(tarjeta, colectivoInterurbano, out Boleto boleto2);
            Assert.AreEqual(8420, boleto2.SaldoRestante);
        }

        [Test]
        public void Trasbordo_EntreInterurbanos_LineaDiferente_EsGratuito()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new Tarjeta { Saldo = 10000 };
            var colectivoInterurbano1 = new Colectivo("Galvez", true);
            var colectivoInterurbano2 = new Colectivo("Baigorria", true);

            colectivoInterurbano1.PagarCon(tarjeta, colectivoInterurbano1, out Boleto boleto1);
            double saldoDespuesPrimero = boleto1.SaldoRestante;
            Assert.AreEqual(7000, saldoDespuesPrimero);

            System.Threading.Thread.Sleep(1000);

            colectivoInterurbano2.PagarCon(tarjeta, colectivoInterurbano2, out Boleto boleto2);
            Assert.AreEqual(7000, boleto2.SaldoRestante);
        }

        [Test]
        public void ColectivoInterurbano_SinFranquicia_AplicaDescuentoFrecuencia()
        {
            var tarjeta = new SinFranquicia { Saldo = 200000 };
            var colectivoInterurbano = new Colectivo("Galvez", true);

            for (int i = 0; i < 30; i++)
            {
                colectivoInterurbano.PagarCon(tarjeta, colectivoInterurbano, out _);
            }

            double saldoAntes = tarjeta.Saldo;
            colectivoInterurbano.PagarCon(tarjeta, colectivoInterurbano, out Boleto boleto);

            double montoEsperado = 3000 * 0.8;
            Assert.AreEqual(saldoAntes - montoEsperado, boleto.SaldoRestante, 0.01);
        }
    }
}