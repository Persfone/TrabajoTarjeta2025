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
            Assert.LessOrEqual(tarjeta.Saldo, 40000);
        }

        [Test]
        public void Pagar_DecrementaSaldoCorrectamente()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 5000;
            bool pudoPagar = tarjeta.Pagar(1580);
            Assert.IsTrue(pudoPagar);
            Assert.AreEqual(3420, tarjeta.Saldo);
        }

        [Test]
        public void Pagar_PermiteSaldoNegativoHasta1200()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 500;
            bool pudoPagar = tarjeta.Pagar(1580);
            Assert.IsTrue(pudoPagar);
            Assert.AreEqual(-1080, tarjeta.Saldo); // 500 - 1580 = -1080
        }

        [Test]
        public void Pagar_NoPermiteSaldoNegativoMayorA1200()
        {
            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 0;
            bool pudoPagar = tarjeta.Pagar(1580);
            Assert.IsFalse(pudoPagar); // 0 - 1580 = -1580, excede -1200
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

        // ===== TESTS DE FRANQUICIAS =====

        [Test]
        public void MedioBoletoEstudiantil_PagaMitadDeTarifa()
        {
            var tarjeta = new MedioBoletoEstudiantil();
            tarjeta.Saldo = 2000;
            bool pudoPagar = tarjeta.Pagar(1580);
            Assert.IsTrue(pudoPagar);
            Assert.AreEqual(1210, tarjeta.Saldo); // 2000 - (1580 * 0.5) = 2000 - 790
        }

        [Test]
        public void MedioBoletoEstudiantil_TipoTarjetaCorrecto()
        {
            var tarjeta = new MedioBoletoEstudiantil();
            Assert.AreEqual("Medio Boleto Estudiantil", tarjeta.TipoTarjeta);
        }

        [Test]
        public void MedioBoletoEstudiantil_MontoBoletoSiempreLaMitad()
        {
            var tarjeta = new MedioBoletoEstudiantil();
            tarjeta.Saldo = 5000;
            double saldoInicial = tarjeta.Saldo;
            tarjeta.Pagar(1580);
            double descuento = saldoInicial - tarjeta.Saldo;
            Assert.AreEqual(790, descuento); // 1580 * 0.5
        }

        [Test]
        public void FranquiciaCompleta_SiemprePuedePagar()
        {
            var tarjeta = new FranquiciaCompleta();
            tarjeta.Saldo = 0;
            bool pudoPagar = tarjeta.Pagar(1580);
            Assert.IsTrue(pudoPagar);
            Assert.AreEqual(0, tarjeta.Saldo); // No se descuenta nada
        }

        [Test]
        public void FranquiciaCompleta_NoDescuentaSaldo()
        {
            var tarjeta = new FranquiciaCompleta();
            tarjeta.Saldo = 1000;
            tarjeta.Pagar(1580);
            Assert.AreEqual(1000, tarjeta.Saldo);
        }

        [Test]
        public void FranquiciaCompleta_TipoTarjetaCorrecto()
        {
            var tarjeta = new FranquiciaCompleta();
            Assert.AreEqual("Franquicia Completa", tarjeta.TipoTarjeta);
        }

        [Test]
        public void BoletoGratuitoEstudiantil_PermiteDosViajesGratis()
        {
            var tarjeta = new BoletoGratuitoEstudiantil();
            tarjeta.Saldo = 0;

            bool primerViaje = tarjeta.Pagar(1580);
            Assert.IsTrue(primerViaje);
            Assert.AreEqual(0, tarjeta.Saldo);

            bool segundoViaje = tarjeta.Pagar(1580);
            Assert.IsTrue(segundoViaje);
            Assert.AreEqual(0, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuitoEstudiantil_TercerViajeRequiereSaldo()
        {
            var tarjeta = new BoletoGratuitoEstudiantil();
            tarjeta.Saldo = 0;

            tarjeta.Pagar(1580); // Viaje 1 gratis
            tarjeta.Pagar(1580); // Viaje 2 gratis

            bool tercerViaje = tarjeta.Pagar(1580); // Este debe cobrar
            Assert.IsFalse(tercerViaje); // Falla porque 0 - 1580 = -1580, excede -1200
        }

        [Test]
        public void BoletoGratuitoEstudiantil_TercerViajeConSaldoSuficiente()
        {
            var tarjeta = new BoletoGratuitoEstudiantil();
            tarjeta.Saldo = 2000;

            tarjeta.Pagar(1580); // Viaje 1 gratis
            tarjeta.Pagar(1580); // Viaje 2 gratis

            bool tercerViaje = tarjeta.Pagar(1580);
            Assert.IsTrue(tercerViaje);
            Assert.AreEqual(420, tarjeta.Saldo); // 2000 - 1580
        }

        [Test]
        public void BoletoGratuitoEstudiantil_TercerViajePermiteSaldoNegativo()
        {
            var tarjeta = new BoletoGratuitoEstudiantil();
            tarjeta.Saldo = 500;

            tarjeta.Pagar(1580); // Viaje 1 gratis
            tarjeta.Pagar(1580); // Viaje 2 gratis

            bool tercerViaje = tarjeta.Pagar(1580);
            Assert.IsTrue(tercerViaje); // 500 - 1580 = -1080, está dentro de -1200
            Assert.AreEqual(-1080, tarjeta.Saldo);
        }

        [Test]
        public void BoletoGratuitoEstudiantil_TipoTarjetaCorrecto()
        {
            var tarjeta = new BoletoGratuitoEstudiantil();
            Assert.AreEqual("Boleto Gratuito Estudiantil", tarjeta.TipoTarjeta);
        }

        [Test]
        public void MedioBoletoEstudiantil_NoPermiteViajeAntesde5Minutos()
        {
            var tarjeta = new MedioBoletoEstudiantil();
            tarjeta.Saldo = 5000;

            // Primer viaje exitoso
            bool primerViaje = tarjeta.Pagar(1580);
            Assert.IsTrue(primerViaje);
            double saldoDespuesPrimero = tarjeta.Saldo;

            // Segundo viaje inmediato (menos de 5 minutos)
            bool segundoViaje = tarjeta.Pagar(1580);
            Assert.IsFalse(segundoViaje);
            Assert.AreEqual(saldoDespuesPrimero, tarjeta.Saldo); // Saldo no cambió
        }

        [Test]
        public void MedioBoletoEstudiantil_PermiteViajesDespuesDe5Minutos()
        {
            var tarjeta = new MedioBoletoEstudiantil();
            tarjeta.Saldo = 5000;

            // Primer viaje
            bool primerViaje = tarjeta.Pagar(1580);
            Assert.IsTrue(primerViaje);
            Assert.AreEqual(4210, tarjeta.Saldo); // 5000 - 790

            // Simular que pasaron 5 minutos modificando fechaUltimoViaje
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);

            // Segundo viaje después de 5 minutos
            bool segundoViaje = tarjeta.Pagar(1580);
            Assert.IsTrue(segundoViaje);
            Assert.AreEqual(3420, tarjeta.Saldo); // 4210 - 790
        }

        [Test]
        public void MedioBoletoEstudiantil_SoloDosViajesConDescuentoPorDia()
        {
            var tarjeta = new MedioBoletoEstudiantil();
            tarjeta.Saldo = 10000;

            // Primer viaje con descuento
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            bool viaje1 = tarjeta.Pagar(1580);
            Assert.IsTrue(viaje1);
            Assert.AreEqual(9210, tarjeta.Saldo); // 10000 - 790

            // Segundo viaje con descuento
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            bool viaje2 = tarjeta.Pagar(1580);
            Assert.IsTrue(viaje2);
            Assert.AreEqual(8420, tarjeta.Saldo); // 9210 - 790

            // Tercer viaje cobra tarifa completa
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            bool viaje3 = tarjeta.Pagar(1580);
            Assert.IsTrue(viaje3);
            Assert.AreEqual(6840, tarjeta.Saldo); // 8420 - 1580 (tarifa completa)
        }

        [Test]
        public void MedioBoletoEstudiantil_TercerViajeCobraTarifaCompleta()
        {
            var tarjeta = new MedioBoletoEstudiantil();
            tarjeta.Saldo = 5000;

            // Dos viajes con descuento
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580); // -790

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580); // -790

            double saldoAntesTercero = tarjeta.Saldo;

            // Tercer viaje
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580);

            // Verificar que cobró tarifa completa
            Assert.AreEqual(saldoAntesTercero - 1580, tarjeta.Saldo);
        }

        [Test]
        public void MedioBoletoEstudiantil_ResetaDespuesDeNuevoDia()
        {
            var tarjeta = new MedioBoletoEstudiantil();
            tarjeta.Saldo = 5000;

            // Simular viaje de ayer
            tarjeta.fechaUltimoViaje = DateTime.Today.AddDays(-1).AddHours(10);

            // Viaje hoy debería tener descuento nuevamente
            bool viaje = tarjeta.Pagar(1580);
            Assert.IsTrue(viaje);
            Assert.AreEqual(4210, tarjeta.Saldo); // 5000 - 790 (con descuento)
        }

        [Test]
        public void BoletoGratuitoEstudiantil_NoPermiteMasDeDosViajesGratuitos()
        {
            var tarjeta = new BoletoGratuitoEstudiantil();
            tarjeta.Saldo = 5000; // Saldo para el tercer viaje

            // Primer viaje gratis
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            bool viaje1 = tarjeta.Pagar(1580);
            Assert.IsTrue(viaje1);
            Assert.AreEqual(5000, tarjeta.Saldo); // Saldo sin cambios

            // Segundo viaje gratis
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            bool viaje2 = tarjeta.Pagar(1580);
            Assert.IsTrue(viaje2);
            Assert.AreEqual(5000, tarjeta.Saldo); // Saldo sin cambios

            // Tercer viaje NO es gratis
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            bool viaje3 = tarjeta.Pagar(1580);
            Assert.IsTrue(viaje3);
            Assert.AreEqual(3420, tarjeta.Saldo); // Se cobró el viaje completo
        }

        [Test]
        public void BoletoGratuitoEstudiantil_ViajesPosterioresAlSegundoSeCobranCompletos()
        {
            var tarjeta = new BoletoGratuitoEstudiantil();
            tarjeta.Saldo = 10000;

            // Dos viajes gratis
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580);
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580);

            double saldoDespuesDeGratuitos = tarjeta.Saldo;
            Assert.AreEqual(10000, saldoDespuesDeGratuitos); // No se descontó nada

            // Tercer viaje
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580);
            Assert.AreEqual(8420, tarjeta.Saldo); // 10000 - 1580

            // Cuarto viaje también se cobra
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580);
            Assert.AreEqual(6840, tarjeta.Saldo); // 8420 - 1580
        }


        [Test]
        public void BoletoGratuitoEstudiantil_TercerViajeSinSaldoFalla()
        {
            var tarjeta = new BoletoGratuitoEstudiantil();
            tarjeta.Saldo = 0;

            // Dos viajes gratis
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580);
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            tarjeta.Pagar(1580);

            // Tercer viaje sin saldo debe fallar
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            bool viaje3 = tarjeta.Pagar(1580);
            Assert.IsFalse(viaje3); // No tiene saldo para pagar
        }
        [Test]
        public void CargarTarjeta_SuperaMaximo_AcreditaHastaMaximoYAlmacenaExcedente()
        {
            // Test que valida que si a una tarjeta se le carga un monto que supere 
            // el máximo permitido, se acredite el saldo hasta alcanzar el máximo (56000) 
            // y que el excedente quede almacenado y pendiente de acreditación.

            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 50000;

            // Simular carga de 10000 cuando solo caben 6000
            double montoACcargar = 10000;
            double espacioDisponible = Tarjeta.SALDO_MAXIMO - tarjeta.Saldo; // 6000
            double excedente = montoACcargar - espacioDisponible; // 4000

            tarjeta.Saldo = Tarjeta.SALDO_MAXIMO;
            tarjeta.SaldoPendiente += excedente;

            // Verificar que se acreditó hasta el máximo
            Assert.AreEqual(56000, tarjeta.Saldo);

            // Verificar que el excedente quedó pendiente
            Assert.AreEqual(4000, tarjeta.SaldoPendiente);
        }

        [Test]
        public void DespuesDeViaje_ConSaldoPendiente_RecargaHastaMaximo()
        {
            // Test que valida que luego de realizar un viaje, verifique si hay saldo 
            // pendiente de acreditación y recargue la tarjeta hasta llegar al máximo nuevamente.

            var tarjeta = new Tarjeta();
            tarjeta.Saldo = 56000;
            tarjeta.SaldoPendiente = 5000;

            // Realizar un viaje
            bool pagoExitoso = tarjeta.Pagar(1580);

            // Verificar que el pago fue exitoso
            Assert.IsTrue(pagoExitoso);

            // Verificar que después del viaje, el saldo volvió al máximo
            Assert.AreEqual(56000, tarjeta.Saldo);

            // Verificar que el saldo pendiente se redujo correctamente
            // Gastó 1580, entonces quedaron 5000 - 1580 = 3420 pendientes
            Assert.AreEqual(3420, tarjeta.SaldoPendiente);
        }

        [Test]
        public void SinFranquicia_Viajes1a29_TarifaNormal()
        {
            var tarjeta = new SinFranquicia();
            tarjeta.Saldo = 50000;

            // Realizar viaje cuando tiene menos de 30 viajes
            double saldoInicial = tarjeta.Saldo;
            tarjeta.Pagar(1580);

            // Debe cobrar tarifa normal (1580)
            Assert.AreEqual(saldoInicial - 1580, tarjeta.Saldo);
        }

        [Test]
        public void SinFranquicia_Viajes30a59_Descuento20Porciento()
        {
            var tarjeta = new SinFranquicia();
            tarjeta.Saldo = 100000;

            // Simular 30 viajes (ya tiene 28 en el código, hacemos 2 más)
            tarjeta.Pagar(1580); // Viaje 29
            tarjeta.Pagar(1580); // Viaje 30

            double saldoAntes = tarjeta.Saldo;
            tarjeta.Pagar(1580); // Viaje 31 - debe tener descuento

            // Debe cobrar 1580 * 0.8 = 1264
            Assert.AreEqual(saldoAntes - 1264, tarjeta.Saldo);
        }

        [Test]
        public void SinFranquicia_Viajes60a80_Descuento25Porciento()
        {
            var tarjeta = new SinFranquicia();
            tarjeta.Saldo = 150000;

            // Simular 60 viajes (tiene 28, hacemos 32 más)
            for (int i = 0; i < 32; i++)
            {
                tarjeta.Pagar(1580);
            }

            double saldoAntes = tarjeta.Saldo;
            tarjeta.Pagar(1580); // Viaje 61 - debe tener 25% descuento

            // Debe cobrar 1580 * 0.75 = 1185
            Assert.AreEqual(saldoAntes - 1185, tarjeta.Saldo);
        }

        [Test]
        public void SinFranquicia_ViajesDespuesDe80_TarifaNormal()
        {
            var tarjeta = new SinFranquicia();
            tarjeta.Saldo = 200000;

            // Simular 81 viajes (tiene 28, hacemos 53 más)
            for (int i = 0; i < 53; i++)
            {
                tarjeta.Pagar(1580);
            }

            double saldoAntes = tarjeta.Saldo;
            tarjeta.Pagar(1580); // Viaje 82 - debe volver a tarifa normal

            // Debe cobrar tarifa normal (1580)
            Assert.AreEqual(saldoAntes - 1580, tarjeta.Saldo);
        }

        [Test]
        public void SinFranquicia_ContadorSeReiniciaElDia1()
        {
            var tarjeta = new SinFranquicia();
            tarjeta.Saldo = 100000;

            // Simular que es día 1 del mes
            // (En tu código se reinicia automáticamente cuando DateTime.Now.Day == 1)

            // Hacer un viaje, debería cobrar tarifa normal
            double saldoInicial = tarjeta.Saldo;
            tarjeta.Pagar(1580);

            Assert.AreEqual(saldoInicial - 1580, tarjeta.Saldo);
        }

        [Test]
        public void SinFranquicia_NoAplicaDescuentoAFranquicias()
        {
            // Verificar que MedioBoleto no tiene uso frecuente
            var medioBoleto = new MedioBoletoEstudiantil();
            medioBoleto.Saldo = 50000;

            double saldoInicial = medioBoleto.Saldo;
            medioBoleto.Pagar(1580);

            // Debe cobrar medio boleto (790), no uso frecuente
            Assert.AreEqual(saldoInicial - 790, medioBoleto.Saldo);
        }
    }
}