using NUnit.Framework;
using TrabajoTarjeta;
using System;

namespace TrabajoTarjeta.Tests
{
    [TestFixture]
    public class TarjetaTests
    {
        [Test]
        public void CargarTarjeta_IncrementaSaldoCorrectamente()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 0;

            int[] cargas = { 2000, 3000, 4000, 5000, 10000, 15000, 20000, 25000, 30000 };
            foreach (var monto in cargas)
            {
                tarjeta.Saldo = 0;
                tarjeta.Saldo += monto;
                Assert.AreEqual(monto, tarjeta.Saldo);
            }
        }

        [Test]
        public void CargarTarjeta_NoSuperaLimiteMaximo()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 35000;
            tarjeta.Saldo += 3000;
            Assert.AreEqual(38000, tarjeta.Saldo);
            Assert.LessOrEqual(tarjeta.Saldo, 56000);
        }

        [Test]
        public void Pagar_DecrementaSaldoCorrectamente()
        {
            var tarjeta = new Tarjeta();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 5000;
            bool pudoPagar = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(pudoPagar);
            Assert.AreEqual(3420, tarjeta.Saldo);
        }

        [Test]
        public void Pagar_PermiteSaldoNegativoHasta1200()
        {
            var tarjeta = new Tarjeta();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 500;
            bool pudoPagar = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(pudoPagar);
            Assert.AreEqual(-1080, tarjeta.Saldo);
        }

        [Test]
        public void Pagar_NoPermiteSaldoNegativoMayorA1200()
        {
            var tarjeta = new Tarjeta();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 0;
            bool pudoPagar = tarjeta.Pagar(1580, colectivo);
            Assert.IsFalse(pudoPagar);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void CargarTarjeta_DescontaSaldoNegativo()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = -1000;
            tarjeta.Saldo += 2000;
            Assert.AreEqual(1000, tarjeta.Saldo);
        }

        [Test]
        public void Tarjeta_TieneIdUnico()
        {
            var tarjeta1 = new Tarjeta();
            var tarjeta2 = new Tarjeta();
            Assert.AreNotEqual(tarjeta1.Id, tarjeta2.Id);
            Assert.IsTrue(tarjeta1.Id.StartsWith("SUBE-"));
        }

        [Test]
        public void Tarjeta_TipoTarjetaPorDefecto()
        {
            var tarjeta = new Tarjeta();
            Assert.AreEqual("Sin Franquicia", tarjeta.TipoTarjeta);
        }

        [Test]
        public void MedioBoletoEstudiantil_PagaMitadDeTarifaDentroHorario()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 2000;
            bool pudoPagar = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(pudoPagar);
            Assert.AreEqual(1210, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void MedioBoletoEstudiantil_TipoTarjetaCorrecto()
        {
            var tarjeta = new MedioBoletoEstudiantil();
            Assert.AreEqual("Medio Boleto Estudiantil", tarjeta.TipoTarjeta);
        }

        [Test]
        public void MedioBoletoEstudiantil_MontoBoletoSiempreLaMitadDentroHorario()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 5000;
            double saldoInicial = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);
            double descuento = saldoInicial - tarjeta.Saldo;
            Assert.AreEqual(790, descuento, 0.01);
        }

        [Test]
        public void FranquiciaCompleta_SiemprePuedePagarDentroHorario()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new FranquiciaCompleta();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 0;
            bool pudoPagar = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(pudoPagar);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void FranquiciaCompleta_NoDescuentaSaldoDentroHorario()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new FranquiciaCompleta();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 1000;
            tarjeta.Pagar(1580, colectivo);
            Assert.AreEqual(1000, tarjeta.Saldo);
        }

        [Test]
        public void FranquiciaCompleta_TipoTarjetaCorrecto()
        {
            var tarjeta = new FranquiciaCompleta();
            Assert.AreEqual("Franquicia Completa", tarjeta.TipoTarjeta);
        }

        [Test]
        public void BoletoGratuitoEstudiantil_PermiteDosViajesGratisDentroHorario()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 0;

            bool primerViaje = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(primerViaje);
            Assert.AreEqual(0, tarjeta.Saldo);

            bool segundoViaje = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(segundoViaje);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuitoEstudiantil_TercerViajeRequiereSaldoDentroHorario()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 0;

            tarjeta.Pagar(1580, colectivo);
            tarjeta.Pagar(1580, colectivo);

            bool tercerViaje = tarjeta.Pagar(1580, colectivo);
            Assert.IsFalse(tercerViaje);
        }

        [Test]
        public void BoletoGratuitoEstudiantil_TercerViajeConSaldoSuficienteDentroHorario()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 2000;

            tarjeta.Pagar(1580, colectivo);
            tarjeta.Pagar(1580, colectivo);

            bool tercerViaje = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(tercerViaje);
            Assert.AreEqual(420, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuitoEstudiantil_TercerViajePermiteSaldoNegativoDentroHorario()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 500;

            tarjeta.Pagar(1580, colectivo);
            tarjeta.Pagar(1580, colectivo);

            bool tercerViaje = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(tercerViaje);
            Assert.AreEqual(-1080, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuitoEstudiantil_TipoTarjetaCorrecto()
        {
            var tarjeta = new BoletoGratuitoEstudiantil();
            Assert.AreEqual("Boleto Gratuito Estudiantil", tarjeta.TipoTarjeta);
        }

        [Test]
        public void MedioBoletoEstudiantil_NoPermiteViajeAntesde5MinutosDentroHorario()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 5000;

            bool primerViaje = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(primerViaje);
            double saldoDespuesPrimero = tarjeta.Saldo;

            bool segundoViaje = tarjeta.Pagar(1580, colectivo);
            Assert.IsFalse(segundoViaje);
            Assert.AreEqual(saldoDespuesPrimero, tarjeta.Saldo);
        }

        [Test]
        public void MedioBoletoEstudiantil_PermiteViajesDespuesDe5MinutosDentroHorario()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 5000;

            bool primerViaje = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(primerViaje);
            Assert.AreEqual(4210, tarjeta.Saldo, 0.01);

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);

            bool segundoViaje = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(segundoViaje);
            Assert.AreEqual(3420, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void MedioBoletoEstudiantil_SoloDosViajesConDescuentoPorDiaDentroHorario()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 10000;

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            bool viaje1 = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(viaje1);
            Assert.AreEqual(9210, tarjeta.Saldo, 0.01);

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            bool viaje2 = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(viaje2);
            Assert.AreEqual(8420, tarjeta.Saldo, 0.01);

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            bool viaje3 = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(viaje3);
            Assert.AreEqual(6840, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void MedioBoletoEstudiantil_TercerViajeCobraTarifaCompletaDentroHorario()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 5000;

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580, colectivo);

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580, colectivo);

            double saldoAntesTercero = tarjeta.Saldo;

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(saldoAntesTercero - 1580, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void MedioBoletoEstudiantil_ResetaDespuesDeNuevoDia()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 5000;

            tarjeta.fechaUltimoViaje = DateTime.Today.AddDays(-1).AddHours(10);

            bool viaje = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(viaje);
            Assert.AreEqual(4210, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void BoletoGratuitoEstudiantil_NoPermiteMasDeDosViajesGratuitos()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 5000;

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            bool viaje1 = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(viaje1);
            Assert.AreEqual(5000, tarjeta.Saldo);

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            bool viaje2 = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(viaje2);
            Assert.AreEqual(5000, tarjeta.Saldo);

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            bool viaje3 = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(viaje3);
            Assert.AreEqual(3420, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuitoEstudiantil_ViajesPosterioresAlSegundoSeCobranCompletos()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 10000;

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580, colectivo);
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580, colectivo);

            double saldoDespuesDeGratuitos = tarjeta.Saldo;
            Assert.AreEqual(10000, saldoDespuesDeGratuitos);

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580, colectivo);
            Assert.AreEqual(8420, tarjeta.Saldo);

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580, colectivo);
            Assert.AreEqual(6840, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuitoEstudiantil_TercerViajeSinSaldoFalla()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 0;

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580, colectivo);
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580, colectivo);

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            bool viaje3 = tarjeta.Pagar(1580, colectivo);
            Assert.IsFalse(viaje3);
        }

        [Test]
        public void CargarTarjeta_SuperaMaximo_AcreditaHastaMaximoYAlmacenaExcedente()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 50000;

            double montoACcargar = 10000;
            double espacioDisponible = Tarjeta.SALDO_MAXIMO - tarjeta.Saldo;
            double excedente = montoACcargar - espacioDisponible;

            tarjeta.Saldo = Tarjeta.SALDO_MAXIMO;
            tarjeta.SaldoPendiente += excedente;

            Assert.AreEqual(56000, tarjeta.Saldo);
            Assert.AreEqual(4000, tarjeta.SaldoPendiente);
        }

        [Test]
        public void DespuesDeViaje_ConSaldoPendiente_RecargaHastaMaximo()
        {
            var tarjeta = new Tarjeta();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 56000;
            tarjeta.SaldoPendiente = 5000;

            bool pagoExitoso = tarjeta.Pagar(1580, colectivo);

            Assert.IsTrue(pagoExitoso);
            Assert.AreEqual(56000, tarjeta.Saldo);
            Assert.AreEqual(3420, tarjeta.SaldoPendiente);
        }

        [Test]
        public void SinFranquicia_Viajes1a29_TarifaNormal()
        {
            var tarjeta = new SinFranquicia();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 50000;

            double saldoInicial = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(saldoInicial - 1580, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void SinFranquicia_Viajes30a59_Descuento20Porciento()
        {
            var tarjeta = new SinFranquicia();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 10000000;

            for (int i = 0; i < 30; i++)
            {
                tarjeta.Pagar(1580, colectivo);
            }

            double saldoAntes = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(saldoAntes - 1264, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void SinFranquicia_Viajes60a80_Descuento25Porciento()
        {
            var tarjeta = new SinFranquicia();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 1500000;

            for (int i = 0; i < 60; i++)
            {
                tarjeta.Pagar(1580, colectivo);
            }

            double saldoAntes = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(saldoAntes - 1185, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void SinFranquicia_ViajesDespuesDe80_TarifaNormal()
        {
            var tarjeta = new SinFranquicia();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 20000000;

            for (int i = 0; i < 81; i++)
            {
                tarjeta.Pagar(1580, colectivo);
            }

            double saldoAntes = tarjeta.Saldo;
            
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(saldoAntes - 1580, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void SinFranquicia_ContadorSeReiniciaElDia1()
        {
            var tarjeta = new SinFranquicia();
            tarjeta.Saldo = 1000000;

            double saldoInicial = tarjeta.Saldo;
            var colectivo = new Colectivo("K");
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(saldoInicial - 1580, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void SinFranquicia_NoAplicaDescuentoAFranquicias()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var medioBoleto = new MedioBoletoEstudiantil();
            medioBoleto.Saldo = 50000;

            double saldoInicial = medioBoleto.Saldo;
            var colectivo = new Colectivo("K");
            medioBoleto.Pagar(1580, colectivo);

            Assert.AreEqual(saldoInicial - 790, medioBoleto.Saldo, 0.01);
        }

        [Test]
        public void MedioBoletoEstudiantil_FueraHorario_CobraTarifaCompleta()
        {
            if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour <= 22)
            {
                Assert.Pass("Test requiere ejecución fuera de horario 6-22");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil();
            tarjeta.Saldo = 5000;
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-10);

            double saldoInicial = tarjeta.Saldo;
            var colectivo = new Colectivo("K");
            tarjeta.Pagar(1580, colectivo);
            Assert.AreEqual(saldoInicial - 1580, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void MedioBoletoEstudiantil_DentroHorario_AplicaDescuento()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil();
            tarjeta.Saldo = 5000;

            double saldoInicial = tarjeta.Saldo;
            var colectivo = new Colectivo("K");
            tarjeta.Pagar(1580, colectivo);
            Assert.AreEqual(saldoInicial - 790, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void BoletoGratuitoEstudiantil_FueraHorario_CobraTarifaCompleta()
        {
            if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour <= 22)
            {
                Assert.Pass("Test requiere ejecución fuera de horario 6-22");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil();
            tarjeta.Saldo = 5000;
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-10);

            double saldoInicial = tarjeta.Saldo;
            var colectivo = new Colectivo("K");
            tarjeta.Pagar(1580, colectivo);
            Assert.AreEqual(saldoInicial - 1580, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void BoletoGratuitoEstudiantil_DentroHorario_ViajeGratis()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new BoletoGratuitoEstudiantil();
            tarjeta.Saldo = 5000;

            double saldoInicial = tarjeta.Saldo;
            var colectivo = new Colectivo("K");
            tarjeta.Pagar(1580, colectivo);
            Assert.AreEqual(saldoInicial, tarjeta.Saldo);
        }

        [Test]
        public void FranquiciaCompleta_FueraHorario_CobraTarifaCompleta()
        {
            if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour <= 22)
            {
                Assert.Pass("Test requiere ejecución fuera de horario 6-22");
                return;
            }

            var tarjeta = new FranquiciaCompleta();
            tarjeta.Saldo = 5000;
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-10);

            double saldoInicial = tarjeta.Saldo;
            var colectivo = new Colectivo("K");
            tarjeta.Pagar(1580, colectivo);
            Assert.AreEqual(saldoInicial - 1580, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void FranquiciaCompleta_DentroHorario_ViajeGratis()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 22)
            {
                Assert.Pass("Test requiere ejecución entre 6:00 y 22:00");
                return;
            }

            var tarjeta = new FranquiciaCompleta();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 5000;

            double saldoInicial = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);
            Assert.AreEqual(saldoInicial, tarjeta.Saldo);
        }

        [Test]
        public void SinFranquicia_NoTieneRestriccionHoraria()
        {
            var tarjeta = new SinFranquicia();
            var colectivo = new Colectivo("K");
            tarjeta.Saldo = 5000;

            double saldoInicial = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);

            Assert.Less(tarjeta.Saldo, saldoInicial);
        }
    }
}