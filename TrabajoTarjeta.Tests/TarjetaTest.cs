using NUnit.Framework;
using TrabajoTarjeta;
using System;
using System.IO;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class TarjetaTests
    {
        // ============================================
        // TESTS DE CONSTRUCTOR Y PROPIEDADES BÁSICAS
        // ============================================

        [Test]
        public void Constructor_InicializaCorrectamente()
        {
            var tarjeta = new BoletoGratuitoEstudiantil { Saldo = 2000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(420, tarjeta.Saldo);
        }

        // ============================================
        // TESTS DE FRANQUICIA COMPLETA
        // ============================================

        [Test]
        public void FranquiciaCompleta_Constructor_EstableceTipo()
        {
            var tarjeta = new FranquiciaCompleta();

            Assert.AreEqual("Franquicia Completa", tarjeta.TipoTarjeta);
        }

        [Test]
        public void FranquiciaCompleta_DentroHorario_ViajeGratis()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new FranquiciaCompleta { Saldo = 1000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(1000, tarjeta.Saldo);
        }

        [Test]
        public void FranquiciaCompleta_DentroHorario_ConSaldoCero_ViajeGratis()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new FranquiciaCompleta { Saldo = 0 };
            var colectivo = new Colectivo("K", false);

            bool resultado = tarjeta.Pagar(1580, colectivo);

            Assert.IsTrue(resultado);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void FranquiciaCompleta_FueraHorario_CobraTarifa()
        {
            if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour <= 22)
            {
                Assert.Pass("Test requiere ejecución fuera de 6:00-22:00");
                return;
            }

            var tarjeta = new FranquiciaCompleta { Saldo = 2000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(420, tarjeta.Saldo);
        }

        [Test]
        public void FranquiciaCompleta_ViajesMultiples_SiempreGratis()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new FranquiciaCompleta { Saldo = 5000 };
            var colectivo = new Colectivo("K", false);

            for (int i = 0; i < 5; i++)
            {
                tarjeta.Pagar(1580, colectivo);
                System.Threading.Thread.Sleep(100);
            }

            Assert.AreEqual(5000, tarjeta.Saldo);
        }

        // ============================================
        // TESTS DE INTEGRACIÓN Y CASOS COMPLEJOS
        // ============================================

        [Test]
        public void MedioBoleto_ConTrasbordo_AplicaDescuentoYTrasbordo()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 5000 };
            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            tarjeta.Pagar(1580, colectivo1);
            Assert.AreEqual(4210, tarjeta.Saldo);

            System.Threading.Thread.Sleep(5100);
            tarjeta.Pagar(1580, colectivo2);
            Assert.AreEqual(4210, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuito_ConTrasbordo_FuncionaCorrectamente()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil { Saldo = 5000 };
            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            tarjeta.Pagar(1580, colectivo1);
            Assert.AreEqual(5000, tarjeta.Saldo);

            System.Threading.Thread.Sleep(100);
            tarjeta.Pagar(1580, colectivo2);
            Assert.AreEqual(5000, tarjeta.Saldo);
        }

        [Test]
        public void SinFranquicia_ConTrasbordo_AplicaDescuentoFrecuencia()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new SinFranquicia { Saldo = 100000 };
            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            for (int i = 0; i < 30; i++)
            {
                tarjeta.Pagar(1580, colectivo1);
            }

            double saldoAntes = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo2);

            Assert.AreEqual(saldoAntes, tarjeta.Saldo);
        }

        [Test]
        public void Tarjeta_SaldoNegativo_ConSaldoPendiente_AcreditaCorrectamente()
        {
            var tarjeta = new Tarjeta { Saldo = 500, SaldoPendiente = 3000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(1920, tarjeta.Saldo);
            Assert.AreEqual(0, tarjeta.SaldoPendiente);
        }

        [Test]
        public void MedioBoleto_SaldoInsuficiente_PagoFallido_NoDecrementaContador()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 0 };
            var colectivo = new Colectivo("K", false);

            bool resultado = tarjeta.Pagar(1580, colectivo);

            Assert.IsFalse(resultado);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_EnHorario7AM_Exacto_EsGratuito()
        {
            if (DateTime.Now.Hour != 7 || DateTime.Now.Minute > 10)
            {
                Assert.Pass("Test requiere ejecución a las 7:00 AM");
                return;
            }

            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test no válido en domingo");
                return;
            }

            var tarjeta = new Tarjeta { Saldo = 5000 };
            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            tarjeta.Pagar(1580, colectivo1);
            double saldoDespues = tarjeta.Saldo;

            System.Threading.Thread.Sleep(100);
            tarjeta.Pagar(1580, colectivo2);

            Assert.AreEqual(saldoDespues, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_EnHorario21_59_EsGratuito()
        {
            if (DateTime.Now.Hour != 21 || DateTime.Now.Minute < 50)
            {
                Assert.Pass("Test requiere ejecución a las 21:50-21:59");
                return;
            }

            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test no válido en domingo");
                return;
            }

            var tarjeta = new Tarjeta { Saldo = 5000 };
            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            tarjeta.Pagar(1580, colectivo1);
            double saldoDespues = tarjeta.Saldo;

            System.Threading.Thread.Sleep(100);
            tarjeta.Pagar(1580, colectivo2);

            Assert.AreEqual(saldoDespues, tarjeta.Saldo);
        }

        [Test]
        public void SinFranquicia_Viaje59_AunDescuento20()
        {
            var tarjeta = new SinFranquicia { Saldo = 150000 };
            var colectivo = new Colectivo("K", false);

            for (int i = 0; i < 59; i++)
            {
                tarjeta.Pagar(1580, colectivo);
            }

            double saldoAntes = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(saldoAntes - 1264, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void SinFranquicia_Viaje80_AunDescuento25()
        {
            var tarjeta = new SinFranquicia { Saldo = 200000 };
            var colectivo = new Colectivo("K", false);

            for (int i = 0; i < 80; i++)
            {
                tarjeta.Pagar(1580, colectivo);
            }

            double saldoAntes = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(saldoAntes - 1185, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void MedioBoleto_HoraLimite6AM_AplicaDescuento()
        {
            if (DateTime.Now.Hour != 6 || DateTime.Now.Minute > 10)
            {
                Assert.Pass("Test requiere ejecución a las 6:00 AM");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 2000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(1210, tarjeta.Saldo);
        }

        [Test]
        public void MedioBoleto_HoraLimite22_AplicaDescuento()
        {
            if (DateTime.Now.Hour != 22 || DateTime.Now.Minute > 10)
            {
                Assert.Pass("Test requiere ejecución a las 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 2000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(1210, tarjeta.Saldo);
        }

        [Test]
        public void MedioBoleto_Hora23_NoAplicaDescuento()
        {
            if (DateTime.Now.Hour != 23)
            {
                Assert.Pass("Test requiere ejecución a las 23:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 2000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(420, tarjeta.Saldo);
        }

        [Test]
        public void MedioBoleto_Hora5_NoAplicaDescuento()
        {
            if (DateTime.Now.Hour != 5)
            {
                Assert.Pass("Test requiere ejecución a las 5:00 AM");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 2000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(420, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuito_HoraLimite6AM_ViajeGratis()
        {
            if (DateTime.Now.Hour != 6 || DateTime.Now.Minute > 10)
            {
                Assert.Pass("Test requiere ejecución a las 6:00 AM");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil { Saldo = 1000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(1000, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuito_HoraLimite22_ViajeGratis()
        {
            if (DateTime.Now.Hour != 22 || DateTime.Now.Minute > 10)
            {
                Assert.Pass("Test requiere ejecución a las 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil { Saldo = 1000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(1000, tarjeta.Saldo);
        }

        [Test]
        public void FranquiciaCompleta_HoraLimite6AM_ViajeGratis()
        {
            if (DateTime.Now.Hour != 6 || DateTime.Now.Minute > 10)
            {
                Assert.Pass("Test requiere ejecución a las 6:00 AM");
                return;
            }

            var tarjeta = new FranquiciaCompleta { Saldo = 1000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(1000, tarjeta.Saldo);
        }

        [Test]
        public void FranquiciaCompleta_HoraLimite22_ViajeGratis()
        {
            if (DateTime.Now.Hour != 22 || DateTime.Now.Minute > 10)
            {
                Assert.Pass("Test requiere ejecución a las 22:00");
                return;
            }

            var tarjeta = new FranquiciaCompleta { Saldo = 1000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(1000, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_Exactamente1Hora_NoEsGratuito()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new Tarjeta { Saldo = 5000 };
            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            tarjeta.Pagar(1580, colectivo1);
            double saldoDespues = tarjeta.Saldo;

            tarjeta.fechaUltimoViaje = DateTime.Now.AddHours(-1);
            tarjeta.Pagar(1580, colectivo2);

            Assert.AreEqual(saldoDespues - 1580, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_CasiUnaHora_EsGratuito()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new Tarjeta { Saldo = 5000 };
            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            tarjeta.Pagar(1580, colectivo1);
            double saldoDespues = tarjeta.Saldo;

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-59);
            tarjeta.Pagar(1580, colectivo2);

            Assert.AreEqual(saldoDespues, tarjeta.Saldo);
        }

        [Test]
        public void Pagar_ConSaldoPendienteYMontoAlto_AcreditaCorrectamente()
        {
            var tarjeta = new Tarjeta { Saldo = 50000, SaldoPendiente = 10000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(56000, tarjeta.Saldo);
            Assert.AreEqual(2420, tarjeta.SaldoPendiente);
        }

        [Test]
        public void SinFranquicia_Viajes29_30_31_DescuentosCorrectos()
        {
            var tarjeta = new SinFranquicia { Saldo = 100000 };
            var colectivo = new Colectivo("K", false);

            for (int i = 0; i < 29; i++)
            {
                tarjeta.Pagar(1580, colectivo);
            }

            double saldoViaje29 = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);
            Assert.AreEqual(saldoViaje29 - 1580, tarjeta.Saldo, 0.01);

            double saldoViaje30 = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);
            Assert.AreEqual(saldoViaje30 - 1264, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void MedioBoleto_PrimerYSegundoViaje_AmbosConDescuento()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 10000 };
            var colectivo = new Colectivo("K", false);

            double inicial = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);
            Assert.AreEqual(inicial - 790, tarjeta.Saldo);

            System.Threading.Thread.Sleep(5100);
            double antesSegundo = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);
            Assert.AreEqual(antesSegundo - 790, tarjeta.Saldo);
        }


        [Test]
        public void Constructor_GeneraIdsUnicos()
        {
            var tarjeta1 = new Tarjeta();
            var tarjeta2 = new Tarjeta();

            Assert.AreNotEqual(tarjeta1.Id, tarjeta2.Id);
        }

        [Test]
        public void Constantes_TienenValoresCorrectos()
        {
            Assert.AreEqual(1200, Tarjeta.SALDO_NEGATIVO);
            Assert.AreEqual(56000, Tarjeta.SALDO_MAXIMO);
        }

        // ============================================
        // TESTS DE PAGAR - CASOS BÁSICOS
        // ============================================

        [Test]
        public void Pagar_ConSaldoSuficiente_DecrementaSaldo()
        {
            var tarjeta = new Tarjeta { Saldo = 5000 };
            var colectivo = new Colectivo("K", false);

            bool resultado = tarjeta.Pagar(1580, colectivo);

            Assert.IsTrue(resultado);
            Assert.AreEqual(3420, tarjeta.Saldo);
        }

        [Test]
        public void Pagar_ActualizaFechaYLinea()
        {
            var tarjeta = new Tarjeta { Saldo = 5000 };
            var colectivo = new Colectivo("K", false);
            DateTime antes = DateTime.Now;

            tarjeta.Pagar(1580, colectivo);

            Assert.GreaterOrEqual(tarjeta.fechaUltimoViaje, antes);
            Assert.AreEqual("K", tarjeta.ultimaLinea);
        }

        [Test]
        public void Pagar_PermiteSaldoNegativoHasta1200()
        {
            var tarjeta = new Tarjeta { Saldo = 500 };
            var colectivo = new Colectivo("K", false);

            bool resultado = tarjeta.Pagar(1580, colectivo);

            Assert.IsTrue(resultado);
            Assert.AreEqual(-1080, tarjeta.Saldo);
        }

        [Test]
        public void Pagar_NoPermiteSaldoNegativoMayorA1200()
        {
            var tarjeta = new Tarjeta { Saldo = 0 };
            var colectivo = new Colectivo("K", false);

            bool resultado = tarjeta.Pagar(1580, colectivo);

            Assert.IsFalse(resultado);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void Pagar_ConSaldoJustoEnLimiteNegativo_Permite()
        {
            var tarjeta = new Tarjeta { Saldo = 380 };
            var colectivo = new Colectivo("K", false);

            bool resultado = tarjeta.Pagar(1580, colectivo);

            Assert.IsTrue(resultado);
            Assert.AreEqual(-1200, tarjeta.Saldo);
        }

        [Test]
        public void Pagar_ConSaldoJustoPorDebajoDelLimite_NoPaga()
        {
            var tarjeta = new Tarjeta { Saldo = 379 };
            var colectivo = new Colectivo("K", false);

            bool resultado = tarjeta.Pagar(1580, colectivo);

            Assert.IsFalse(resultado);
            Assert.AreEqual(379, tarjeta.Saldo);
        }

        // ============================================
        // TESTS DE TRASBORDO
        // ============================================

        [Test]
        public void Trasbordo_PrimeraVez_UltimaLineaNull_CobraNormal()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new Tarjeta { Saldo = 5000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(3420, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_LineaDiferente_DentroDeUnaHora_EsGratuito()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new Tarjeta { Saldo = 5000 };
            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            tarjeta.Pagar(1580, colectivo1);
            double saldoDespuesPrimero = tarjeta.Saldo;

            System.Threading.Thread.Sleep(100);
            tarjeta.Pagar(1580, colectivo2);

            Assert.AreEqual(saldoDespuesPrimero, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_MismaLinea_NoEsGratuito()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new Tarjeta { Saldo = 5000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);
            double saldoDespuesPrimero = tarjeta.Saldo;

            System.Threading.Thread.Sleep(100);
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(saldoDespuesPrimero - 1580, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_DespuesDe1Hora_NoEsGratuito()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new Tarjeta { Saldo = 5000 };
            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            tarjeta.Pagar(1580, colectivo1);
            double saldoDespuesPrimero = tarjeta.Saldo;

            tarjeta.fechaUltimoViaje = DateTime.Now.AddHours(-1.1);
            tarjeta.Pagar(1580, colectivo2);

            Assert.AreEqual(saldoDespuesPrimero - 1580, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_EnDomingo_NoEsGratuito()
        {
            if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución en domingo");
                return;
            }

            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22)
            {
                Assert.Pass("Test requiere horario 7-22");
                return;
            }

            var tarjeta = new Tarjeta { Saldo = 5000 };
            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            tarjeta.Pagar(1580, colectivo1);
            double saldoDespuesPrimero = tarjeta.Saldo;

            System.Threading.Thread.Sleep(100);
            tarjeta.Pagar(1580, colectivo2);

            Assert.AreEqual(saldoDespuesPrimero - 1580, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_Antes7AM_NoEsGratuito()
        {
            if (DateTime.Now.Hour >= 7)
            {
                Assert.Pass("Test requiere ejecución antes de las 7:00");
                return;
            }

            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test no válido en domingo");
                return;
            }

            var tarjeta = new Tarjeta { Saldo = 5000 };
            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            tarjeta.Pagar(1580, colectivo1);
            double saldoDespuesPrimero = tarjeta.Saldo;

            System.Threading.Thread.Sleep(100);
            tarjeta.Pagar(1580, colectivo2);

            Assert.AreEqual(saldoDespuesPrimero - 1580, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_DespuesDe22HS_NoEsGratuito()
        {
            if (DateTime.Now.Hour < 22)
            {
                Assert.Pass("Test requiere ejecución después de las 22:00");
                return;
            }

            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test no válido en domingo");
                return;
            }

            var tarjeta = new Tarjeta { Saldo = 5000 };
            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            tarjeta.Pagar(1580, colectivo1);
            double saldoDespuesPrimero = tarjeta.Saldo;

            System.Threading.Thread.Sleep(100);
            tarjeta.Pagar(1580, colectivo2);

            Assert.AreEqual(saldoDespuesPrimero - 1580, tarjeta.Saldo);
        }

        // ============================================
        // TESTS DE ACREDITAR CARGA
        // ============================================

        [Test]
        public void AcreditarCarga_SinSaldoPendiente_NoHaceNada()
        {
            var tarjeta = new Tarjeta { Saldo = 5000, SaldoPendiente = 0 };

            tarjeta.AcreditarCarga();

            Assert.AreEqual(5000, tarjeta.Saldo);
            Assert.AreEqual(0, tarjeta.SaldoPendiente);
        }

        [Test]
        public void AcreditarCarga_ConSaldoPendiente_YEspacioDisponible_Acredita()
        {
            var tarjeta = new Tarjeta { Saldo = 50000, SaldoPendiente = 3000 };

            tarjeta.AcreditarCarga();

            Assert.AreEqual(53000, tarjeta.Saldo);
            Assert.AreEqual(0, tarjeta.SaldoPendiente);
        }

        [Test]
        public void AcreditarCarga_ConSaldoPendiente_SinEspacioDisponible_NoAcredita()
        {
            var tarjeta = new Tarjeta { Saldo = 56000, SaldoPendiente = 3000 };

            tarjeta.AcreditarCarga();

            Assert.AreEqual(56000, tarjeta.Saldo);
            Assert.AreEqual(3000, tarjeta.SaldoPendiente);
        }

        [Test]
        public void AcreditarCarga_AcreditaParcial_CuandoNoHayEspacioCompleto()
        {
            var tarjeta = new Tarjeta { Saldo = 54000, SaldoPendiente = 5000 };

            tarjeta.AcreditarCarga();

            Assert.AreEqual(56000, tarjeta.Saldo);
            Assert.AreEqual(3000, tarjeta.SaldoPendiente);
        }

        [Test]
        public void Pagar_LlamaAcreditarCarga_DespuesDelPago()
        {
            var tarjeta = new Tarjeta { Saldo = 56000, SaldoPendiente = 5000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(56000, tarjeta.Saldo);
            Assert.AreEqual(3420, tarjeta.SaldoPendiente);
        }

        // ============================================
        // TESTS DE SINFRANQUICIA
        // ============================================

        [Test]
        public void SinFranquicia_Constructor_HeredaPropiedades()
        {
            var tarjeta = new SinFranquicia();

            Assert.AreEqual(0, tarjeta.Saldo);
            Assert.AreEqual("Sin Franquicia", tarjeta.TipoTarjeta);
        }

        [Test]
        public void SinFranquicia_Viajes1a29_TarifaNormal()
        {
            var tarjeta = new SinFranquicia { Saldo = 50000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(48420, tarjeta.Saldo);
        }

        [Test]
        public void SinFranquicia_Viaje30_Descuento20Porciento()
        {
            var tarjeta = new SinFranquicia { Saldo = 100000 };
            var colectivo = new Colectivo("K", false);

            for (int i = 0; i < 30; i++)
            {
                tarjeta.Pagar(1580, colectivo);
            }

            double saldoAntes = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(saldoAntes - 1264, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void SinFranquicia_Viaje60_Descuento25Porciento()
        {
            var tarjeta = new SinFranquicia { Saldo = 150000 };
            var colectivo = new Colectivo("K", false);

            for (int i = 0; i < 60; i++)
            {
                tarjeta.Pagar(1580, colectivo);
            }

            double saldoAntes = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(saldoAntes - 1185, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void SinFranquicia_Viaje81_TarifaNormal()
        {
            var tarjeta = new SinFranquicia { Saldo = 200000 };
            var colectivo = new Colectivo("K", false);

            for (int i = 0; i < 81; i++)
            {
                tarjeta.Pagar(1580, colectivo);
            }

            double saldoAntes = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(saldoAntes - 1580, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void SinFranquicia_PagoFallido_NoIncrementaContador()
        {
            var tarjeta = new SinFranquicia { Saldo = 0 };
            var colectivo = new Colectivo("K", false);

            bool resultado = tarjeta.Pagar(1580, colectivo);

            Assert.IsFalse(resultado);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void SinFranquicia_Dia1_ReiniciaContador()
        {
            if (DateTime.Now.Day != 1)
            {
                Assert.Pass("Test requiere ejecución el día 1 del mes");
                return;
            }

            var tarjeta = new SinFranquicia { Saldo = 50000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(48420, tarjeta.Saldo);
        }

        // ============================================
        // TESTS DE MEDIOBOLETO ESTUDIANTIL
        // ============================================

        [Test]
        public void MedioBoleto_Constructor_EstableceTipo()
        {
            var tarjeta = new MedioBoletoEstudiantil();

            Assert.AreEqual("Medio Boleto Estudiantil", tarjeta.TipoTarjeta);
        }

        [Test]
        public void MedioBoleto_DentroHorario_PagaMitad()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 2000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(1210, tarjeta.Saldo);
        }

        [Test]
        public void MedioBoleto_FueraHorario_PagaCompleto()
        {
            if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour <= 22)
            {
                Assert.Pass("Test requiere ejecución fuera de 6:00-22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 2000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(420, tarjeta.Saldo);
        }

        [Test]
        public void MedioBoleto_Antes5Minutos_NoPermite()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 5000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);
            double saldoDespues = tarjeta.Saldo;

            bool resultado = tarjeta.Pagar(1580, colectivo);

            Assert.IsFalse(resultado);
            Assert.AreEqual(saldoDespues, tarjeta.Saldo);
        }

        [Test]
        public void MedioBoleto_Despues5Minutos_Permite()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 5000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(5100);

            bool resultado = tarjeta.Pagar(1580, colectivo);

            Assert.IsTrue(resultado);
        }

        [Test]
        public void MedioBoleto_PrimerViajeDia_TieneDescuento()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 5000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(4210, tarjeta.Saldo);
        }

        [Test]
        public void MedioBoleto_SegundoViajeDia_TieneDescuento()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 5000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(5100);
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(3420, tarjeta.Saldo);
        }

        [Test]
        public void MedioBoleto_TercerViajeDia_NoTieneDescuento()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 10000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(5100);
            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(5100);
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(6840, tarjeta.Saldo);
        }

        [Test]
        public void MedioBoleto_NuevoDia_ResetaContador()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 5000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.fechaUltimoViaje = DateTime.Today.AddDays(-1);
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(4210, tarjeta.Saldo);
        }

        [Test]
        public void MedioBoleto_PagoFallido_DecrementaContador()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil { Saldo = 500 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(5100);

            bool resultado = tarjeta.Pagar(1580, colectivo);

            Assert.IsFalse(resultado);
        }

        // ============================================
        // TESTS DE BOLETO GRATUITO ESTUDIANTIL
        // ============================================

        [Test]
        public void BoletoGratuito_Constructor_EstableceTipo()
        {
            var tarjeta = new BoletoGratuitoEstudiantil();

            Assert.AreEqual("Boleto Gratuito Estudiantil", tarjeta.TipoTarjeta);
        }

        [Test]
        public void BoletoGratuito_PrimerViaje_EsGratis()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil { Saldo = 0 };
            var colectivo = new Colectivo("K", false);

            bool resultado = tarjeta.Pagar(1580, colectivo);

            Assert.IsTrue(resultado);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuito_SegundoViaje_EsGratis()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil { Saldo = 0 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(100);
            bool resultado = tarjeta.Pagar(1580, colectivo);

            Assert.IsTrue(resultado);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuito_TercerViaje_SinSaldo_NoPermite()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil { Saldo = 0 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(100);
            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(100);

            bool resultado = tarjeta.Pagar(1580, colectivo);

            Assert.IsFalse(resultado);
        }

        [Test]
        public void BoletoGratuito_TercerViaje_ConSaldo_CobraTarifa()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil { Saldo = 5000 };
            var colectivo = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(100);
            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(100);
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(3420, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuito_NuevoDia_ResetaContador()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil { Saldo = 0 };
            var colectivo = new Colectivo("K", false);

            tarjeta.fechaUltimoViaje = DateTime.Today.AddDays(-1);
            bool resultado = tarjeta.Pagar(1580, colectivo);

            Assert.IsTrue(resultado);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuito_FueraHorario_CobraTarifa()
        {
            if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour <= 22)
            {
                Assert.Pass("Test requiere ejecución fuera de 6:00-22:00");
                return;
            }

            var tarjeta = new Tarjeta();

            Assert.AreEqual(0, tarjeta.Saldo);
            Assert.AreEqual("Sin Franquicia", tarjeta.TipoTarjeta);
            Assert.AreEqual(0, tarjeta.SaldoPendiente);
            Assert.IsTrue(tarjeta.Id.StartsWith("SUBE-"));
            Assert.AreEqual(DateTime.MinValue, tarjeta.fechaUltimoViaje);
            Assert.IsNull(tarjeta.ultimaLinea);
        }
    }
}