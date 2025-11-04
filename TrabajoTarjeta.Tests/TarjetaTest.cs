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
        public void BoletoGratuitoEstudiantil_NoPermiteUsoAntesde5Minutos()
        {
            var tarjeta = new BoletoGratuitoEstudiantil();
            tarjeta.Saldo = 5000;

            // Primer viaje gratis
            bool viaje1 = tarjeta.Pagar(1580);
            Assert.IsTrue(viaje1);

            // Intento inmediato (sin esperar 5 minutos)
            bool viaje2 = tarjeta.Pagar(1580);
            Assert.IsFalse(viaje2); // Debe rechazar
            Assert.AreEqual(5000, tarjeta.Saldo); // Saldo no cambió
        }

        [Test]
        public void BoletoGratuitoEstudiantil_PermiteUsoDepusDe5Minutos()
        {
            var tarjeta = new BoletoGratuitoEstudiantil();
            tarjeta.Saldo = 5000;

            // Primer viaje gratis
            bool viaje1 = tarjeta.Pagar(1580);
            Assert.IsTrue(viaje1);

            // Simular que pasaron 5 minutos
            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);

            // Segundo viaje gratis
            bool viaje2 = tarjeta.Pagar(1580);
            Assert.IsTrue(viaje2);
            Assert.AreEqual(5000, tarjeta.Saldo); // Aún gratis
        }

        [Test]
        public void BoletoGratuitoEstudiantil_ContadorSeReiniciaAlDiaSiguiente()
        {
            var tarjeta = new BoletoGratuitoEstudiantil();
            tarjeta.Saldo = 5000;

            // Simular que el último viaje fue ayer
            tarjeta.fechaUltimoViaje = DateTime.Today.AddDays(-1).AddHours(10);

            // Hoy debería tener 2 viajes gratis nuevamente
            bool viaje1 = tarjeta.Pagar(1580);
            Assert.IsTrue(viaje1);
            Assert.AreEqual(5000, tarjeta.Saldo); // Gratis

            tarjeta.fechaUltimoViaje = DateTime.Now.AddMinutes(-6);
            bool viaje2 = tarjeta.Pagar(1580);
            Assert.IsTrue(viaje2);
            Assert.AreEqual(5000, tarjeta.Saldo); // Gratis
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
    }
}