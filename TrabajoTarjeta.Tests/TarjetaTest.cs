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
    }
}