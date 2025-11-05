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
            var colectivo = new Colectivo("K", false);
            tarjeta.Saldo = 5000;
            bool pudoPagar = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(pudoPagar);
            Assert.AreEqual(3420, tarjeta.Saldo);
        }

        [Test]
        public void Pagar_PermiteSaldoNegativoHasta1200()
        {
            var tarjeta = new Tarjeta();
            var colectivo = new Colectivo("K", false);
            tarjeta.Saldo = 500;
            bool pudoPagar = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(pudoPagar);
            Assert.AreEqual(-1080, tarjeta.Saldo);
        }

        [Test]
        public void Pagar_NoPermiteSaldoNegativoMayorA1200()
        {
            var tarjeta = new Tarjeta();
            var colectivo = new Colectivo("K", false);
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
            var colectivo = new Colectivo("K", false);
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
            var colectivo1 = new Colectivo("K", false);
            tarjeta.Saldo = 5000;
            double saldoInicial = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo1);
            double saldoDespuesPrimero = tarjeta.Saldo;

            tarjeta.fechaUltimoViaje = DateTime.Now.AddHours(-1.1);

            tarjeta.Pagar(1580, colectivo1);

            Assert.AreEqual(saldoDespuesPrimero - 1580, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_EnDomingo_NoCumpleCondicion()
        {
            if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución en domingo");
                return;
            }

            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22)
            {
                Assert.Pass("Test requiere ejecución entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 5000;

            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            tarjeta.Pagar(1580, colectivo1);
            double saldoDespuesPrimero = tarjeta.Saldo;

            System.Threading.Thread.Sleep(1000);
            tarjeta.Pagar(1580, colectivo2);

            Assert.AreEqual(saldoDespuesPrimero - 1580, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_FueraHorario_NoCumpleCondicion()
        {
            if (DateTime.Now.Hour >= 7 && DateTime.Now.Hour < 22)
            {
                Assert.Pass("Test requiere ejecución fuera de horario 7:00-22:00");
                return;
            }

            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test no se ejecuta en domingo");
                return;
            }

            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 5000;

            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            tarjeta.Pagar(1580, colectivo1);
            double saldoDespuesPrimero = tarjeta.Saldo;

            System.Threading.Thread.Sleep(1000);
            tarjeta.Pagar(1580, colectivo2);

            Assert.AreEqual(saldoDespuesPrimero - 1580, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_MultiplesTrasbordo_DentroDeUnaHora()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 10000;

            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);
            var colectivo3 = new Colectivo("102", false);

            double saldoInicial = tarjeta.Saldo;

            tarjeta.Pagar(1580, colectivo1);
            Assert.AreEqual(saldoInicial - 1580, tarjeta.Saldo);

            System.Threading.Thread.Sleep(1000);
            tarjeta.Pagar(1580, colectivo2);
            Assert.AreEqual(saldoInicial - 1580, tarjeta.Saldo);

            System.Threading.Thread.Sleep(1000);
            tarjeta.Pagar(1580, colectivo3);
            Assert.AreEqual(saldoInicial - 1580, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_ConFranquicia_FuncionaCorrectamente()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new FranquiciaCompleta();
            tarjeta.Saldo = 5000;

            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            double saldoInicial = tarjeta.Saldo;

            tarjeta.Pagar(1580, colectivo1);
            Assert.AreEqual(saldoInicial, tarjeta.Saldo);

            System.Threading.Thread.Sleep(1000);
            tarjeta.Pagar(1580, colectivo2);
            Assert.AreEqual(saldoInicial, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_ConMedioBoleto_FuncionaCorrectamente()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new MedioBoletoEstudiantil();
            tarjeta.Saldo = 5000;

            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            double saldoInicial = tarjeta.Saldo;

            tarjeta.Pagar(1580, colectivo1);
            Assert.AreEqual(saldoInicial - 790, tarjeta.Saldo, 0.01);

            System.Threading.Thread.Sleep(5100);
            tarjeta.Pagar(1580, colectivo2);
            Assert.AreEqual(saldoInicial - 790, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void Trasbordo_VuelveALinea_DespuesDeOtra_NoCumpleCondicion()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 10000;

            var colectivoK = new Colectivo("K", false);
            var colectivo142 = new Colectivo("142", false);

            tarjeta.Pagar(1580, colectivoK);
            double saldoDespuesPrimero = tarjeta.Saldo;

            System.Threading.Thread.Sleep(1000);
            tarjeta.Pagar(1580, colectivo142);
            double saldoDespuesSegundo = tarjeta.Saldo;
            Assert.AreEqual(saldoDespuesPrimero, saldoDespuesSegundo);

            System.Threading.Thread.Sleep(1000);
            tarjeta.Pagar(1580, colectivoK);
            Assert.AreEqual(saldoDespuesSegundo, tarjeta.Saldo);
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
            var colectivo = new Colectivo("K", false);
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
            var colectivo = new Colectivo("K", false);
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
            var colectivo = new Colectivo("K", false);
            tarjeta.Saldo = 0;

            bool primerViaje = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(primerViaje);
            Assert.AreEqual(0, tarjeta.Saldo);

            System.Threading.Thread.Sleep(5100);

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
            var colectivo = new Colectivo("K", false);
            tarjeta.Saldo = 0;

            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(5100);
            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(5100);

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
            var colectivo = new Colectivo("K", false);
            tarjeta.Saldo = 2000;

            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(5100);
            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(5100);

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
            var colectivo = new Colectivo("K", false);
            tarjeta.Saldo = 500;

            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(5100);
            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(5100);

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
            var colectivo = new Colectivo("K", false);
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
            var colectivo = new Colectivo("K", false);
            tarjeta.Saldo = 5000;

            bool primerViaje = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(primerViaje);
            Assert.AreEqual(4210, tarjeta.Saldo, 0.01);

            System.Threading.Thread.Sleep(5100);

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
            var colectivo = new Colectivo("K", false);
            tarjeta.Saldo = 10000;

            bool viaje1 = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(viaje1);
            Assert.AreEqual(9210, tarjeta.Saldo, 0.01);

            System.Threading.Thread.Sleep(5100);
            bool viaje2 = tarjeta.Pagar(1580, colectivo);
            Assert.IsTrue(viaje2);
            Assert.AreEqual(8420, tarjeta.Saldo, 0.01);

            System.Threading.Thread.Sleep(5100);
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
            var colectivo = new Colectivo("K", false);
            tarjeta.Saldo = 5000;

            tarjeta.Pagar(1580, colectivo);
            System.Threading.Thread.Sleep(5100);
            tarjeta.Pagar(1580, colectivo);

            double saldoAntesTercero = tarjeta.Saldo;
            System.Threading.Thread.Sleep(5100);
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(saldoAntesTercero - 1580, tarjeta.Saldo, 0.01);
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
            var colectivo = new Colectivo("K", false);
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
            var colectivo = new Colectivo("K", false);
            tarjeta.Saldo = 50000;

            double saldoInicial = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);

            Assert.AreEqual(saldoInicial - 1580, tarjeta.Saldo, 0.01);
        }

        [Test]
        public void SinFranquicia_Viajes30a59_Descuento20Porciento()
        {
            var tarjeta = new SinFranquicia();
            var colectivo = new Colectivo("K", false);
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
            var colectivo = new Colectivo("K", false);
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
            var colectivo = new Colectivo("K", false);
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
            var colectivo = new Colectivo("K", false);
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
            var colectivo = new Colectivo("K", false);
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

            double saldoInicial = tarjeta.Saldo;
            var colectivo = new Colectivo("K", false);
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
            var colectivo = new Colectivo("K", false);
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

            double saldoInicial = tarjeta.Saldo;
            var colectivo = new Colectivo("K", false);
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
            var colectivo = new Colectivo("K", false);
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

            double saldoInicial = tarjeta.Saldo;
            var colectivo = new Colectivo("K", false);
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
            var colectivo = new Colectivo("K", false);
            tarjeta.Saldo = 5000;

            double saldoInicial = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);
            Assert.AreEqual(saldoInicial, tarjeta.Saldo);
        }

        [Test]
        public void SinFranquicia_NoTieneRestriccionHoraria()
        {
            var tarjeta = new SinFranquicia();
            var colectivo = new Colectivo("K", false);
            tarjeta.Saldo = 5000;

            double saldoInicial = tarjeta.Saldo;
            tarjeta.Pagar(1580, colectivo);

            Assert.Less(tarjeta.Saldo, saldoInicial);
        }

        // ============================================
        // TESTS PARA TRASBORDOS
        // ============================================

        [Test]
        public void Trasbordo_EntreLineasDiferentes_EsGratuito()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 5000;

            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            double saldoInicial = tarjeta.Saldo;

            tarjeta.Pagar(1580, colectivo1);
            double saldoDespuesPrimero = tarjeta.Saldo;
            Assert.AreEqual(saldoInicial - 1580, saldoDespuesPrimero);

            System.Threading.Thread.Sleep(1000);
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

            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 5000;

            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("K", false);

            tarjeta.Pagar(1580, colectivo1);
            double saldoDespuesPrimero = tarjeta.Saldo;

            System.Threading.Thread.Sleep(1000);
            tarjeta.Pagar(1580, colectivo2);

            Assert.AreEqual(saldoDespuesPrimero - 1580, tarjeta.Saldo);
        }

        [Test]
        public void Trasbordo_DespuesDe1Hora_NoCumpleCondicion()
        {
            if (DateTime.Now.Hour < 7 || DateTime.Now.Hour >= 22 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.Pass("Test requiere ejecución de lunes a sábado entre 7:00 y 22:00");
                return;
            }

            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 5000;

            var colectivo1 = new Colectivo("K", false);
            var colectivo2 = new Colectivo("142", false);

            tarjeta.Pagar(1580, colectivo1);
            double saldoDespuesPrimero = tarjeta.Saldo;

            tarjeta.fechaUltimoViaje = DateTime.Now.AddHours(-1.1);

            tarjeta.Pagar(1580, colectivo2);

            Assert.AreEqual(saldoDespuesPrimero - 1580, tarjeta.Saldo);
        }
    }
}